import express from 'express';
import PaymentDetails from './PaymentDetails.js';
import OrdersModel from './OrdersModels.js';
import bcrypt from 'bcrypt';

const router = express.Router();


router.post('/payment', async (req, res) => {
    try {
        const { account_id, order_id, amount, cardNumber, expDate, cvv } = req.body;

        const saltRounds = 10;
        const hashedCardNumber = await bcrypt.hash(cardNumber, saltRounds);

        const newPayment = await PaymentDetails.create({
            account_id,
            order_id,
            amount,
            cardNumber: hashedCardNumber,
            expDate,
            cvv
        });

        await OrdersModel.update(
            { status: 'rented' },
            { where: { account_id: account_id, order_id: order_id } }
        );

        res.status(201).json(newPayment);
    } catch (error) {
        res.status(500).json({ message: 'Error processing payment', error: error.message });
    }
});

export default router;
