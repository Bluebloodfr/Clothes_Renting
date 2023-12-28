import express from 'express';
import userRoutes from './UserRoutes.js';
import clothesRoutes from './clothesRoutes.js';
import jwt from 'jsonwebtoken';
import AccountsModels from './AccountsModels.js';
import paymentDetailsRoutes from './PaymentDetailsRoutes.js';
import path from 'path';
import cors from 'cors';

const app = express();

app.use(cors());

app.use(express.json());
app.use(express.static('public'));

app.get('/', (req, res) => {
    res.sendFile(path.join(process.cwd(), 'public/landing.html'));
});

app.use('/api/users', userRoutes);
app.use('/api/clothes', clothesRoutes);
app.use('/api/pay', paymentDetailsRoutes);
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Server running on port ${PORT}`));