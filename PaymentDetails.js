import { Model, DataTypes } from 'sequelize';
import sequelize from './sequelize.js';

export class PaymentDetails extends Model {}

PaymentDetails.init({
    payment_id: {
        type: DataTypes.INTEGER,
        primaryKey: true,
        autoIncrement: true
    },
    account_id: {
        type: DataTypes.INTEGER,
        allowNull: false
    },
    order_id: {
        type: DataTypes.INTEGER,
        allowNull: false
    },
    amount: {
        type: DataTypes.DECIMAL(10, 2),
        allowNull: false
    },
    cardNumber: {
        type: DataTypes.STRING,
        allowNull: false
    },
    expDate: {
        type: DataTypes.STRING,
        allowNull: false
    },
    cvv: {
        type: DataTypes.STRING,
        allowNull: false
    },
}, {
    sequelize,
    modelName: 'PaymentDetails'
});

export default PaymentDetails;