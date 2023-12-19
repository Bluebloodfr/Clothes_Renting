import express from 'express';
import bcrypt from 'bcrypt';
import jwt from 'jsonwebtoken';
import AccountsModels from './AccountsModels.js'; // Using AccountsModels
import path from 'path';

const router = express.Router();

// User signup route
router.post('/signup', async (req, res) => {
    try {
        const { username, email, password, role } = req.body;

        const existingUser = await AccountsModels.findOne({ where: { email } });
        if (existingUser) {
            return res.status(400).json({ message: 'Email already exists' });
        }

        const hashedPassword = await bcrypt.hash(password, 10);

        const userRole = role || 'customer';

        const newUser = await AccountsModels.create({
            username,
            email,
            password_hash: hashedPassword,
            role: userRole // Assign the userRole here
        });

        res.status(201).json({ message: 'User created successfully', userId: newUser.account_id });
    } catch (error) {
        console.error('Error creating new user:', error);
        res.status(500).json({ message: 'Error creating new user' });
    }
});

router.post('/login', async (req, res) => {
    try {
        const { email, password } = req.body;
        const user = await AccountsModels.findOne({ where: { email } });

        if (!user) {
            return res.status(401).send('Authentication failed: user not found');
        }

        const isMatch = await bcrypt.compare(password, user.password_hash);
        if (!isMatch) {
            return res.status(401).send('Authentication failed: incorrect password');
        }
        // Generate a token with user role information
        const token = jwt.sign(
            { userId: user.account_id, email: user.email, role: user.role },
            'abc',
            { expiresIn: '1h' }
        );
        res.status(200).json({
            message: 'Logged in successfully',
            token,
            userRole: user.role
        });
    } catch (error) {
        console.error(error);
        res.status(500).send('Error logging in');
    }
});




export default router;