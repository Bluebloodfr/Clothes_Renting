import express from 'express';
import bcrypt from 'bcrypt';
import jwt from 'jsonwebtoken';
import AccountsModels from './AccountsModels.js'; // Using AccountsModels
import ClothesModel from './ClothesModels.js';
import OrdersModel from './OrdersModels.js';

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
            role: userRole
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

router.post('/checkout', async (req, res) => {
    try {
        const { userId, cartItems, rentalEnd } = req.body;

        const rentalStart = new Date();

        const newOrder = await OrdersModel.create({
            account_id: userId,
            status: 'pending',
            rental_start_date: rentalStart,
            rental_end_date: rentalEnd
        });
        for (const item of cartItems) {
            await ClothesModel.update(
                { order_id: newOrder.order_id },
                { where: { clothes_id: item.clothes_id } }
            );
        }
        res.status(201).json({ message: "Order created successfully", order_id: newOrder.order_id });
    } catch (error) {
        console.error('Error processing checkout:', error);
        res.status(500).json({ message: 'Error processing checkout' });
    }
});

router.get('/orders/:account_id', async (req, res) => {
    try {
        const account_id = req.params.account_id;
        const userOrders = await OrdersModel.findAll({ where: { account_id: account_id } });

        const ordersWithClothes = await Promise.all(userOrders.map(async (order) => {
            const clothes = await ClothesModel.findAll({ where: { order_id: order.order_id } });
            return { ...order.toJSON(), clothes };
        }));

        res.json(ordersWithClothes);
    } catch (error) {
        console.error('Error occurred:', error);
        res.status(500).send('Server error');
    }
});
router.post('/updateProfile', async (req, res) => {
    try {
        const { account_id, username, email } = req.body;
        const updateData = {};
        if (username) updateData.username = username;
        if (email) updateData.email = email;

        const [updatedRows] = await AccountsModels.update(
            updateData,
            { where: { account_id: account_id } }
        );

        if (updatedRows > 0) {
            res.json({ message: 'Profile updated successfully' });
        } else {
            res.status(404).send('User not found');
        }
    } catch (error) {
        res.status(500).send('Error updating profile');
    }
});

router.post('/changePassword', async (req, res) => {
    try {
        const { account_id, currentPassword, newPassword } = req.body;
        const user = await AccountsModels.findOne({ where: { account_id: account_id } });

        if (!user) {
            return res.status(404).send('User not found');
        }

        const match = await bcrypt.compare(currentPassword, user.password_hash);
        if (!match) {
            return res.status(401).send('Incorrect current password');
        }

        const hashedPassword = await bcrypt.hash(newPassword, 10);
        await AccountsModels.update({ password_hash: hashedPassword }, { where: { account_id: account_id } });
        res.send('Password updated successfully');
    } catch (error) {
        res.status(500).send('Error changing password');
    }
});

router.delete('/orders/remove/:order_id', async (req, res) => {
    try {
        await OrdersModel.destroy({ where: { order_id: req.params.order_id } });
        res.send('Order removed successfully');
    } catch (error) {
        res.status(500).send('Error removing order');
    }
});

export default router;