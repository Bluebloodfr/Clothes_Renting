import express from 'express';
import userRoutes from './UserRoutes.js';
import clothesRoutes from './clothesRoutes.js';
import jwt from 'jsonwebtoken';
import AccountsModels from './AccountsModels.js';
import path from 'path';
import cors from 'cors';

const app = express();

app.use(cors());

app.use(express.json());
app.use(express.static('public'));

app.get('/', (req, res) => {
    res.sendFile(path.join(process.cwd(), 'public/landing.html'));
});

const isAdmin = (req, res, next) => {
    try {
        const token = req.headers.authorization.split(' ')[1];
        const decodedToken = jwt.verify(token, 'abc');

        AccountsModels.findByPk(decodedToken.userId).then(user => {
            if (user && user.role === 'admin') {
                next();
            } else {
                res.status(403).json({ message: 'Access denied' });
            }
        });
    } catch {
        res.status(401).json({ message: 'Invalid or missing token' });
    }
};

// Routes
app.use('/api/users', userRoutes);
app.use('/api/clothes', clothesRoutes);

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Server running on port ${PORT}`));