import { Model, DataTypes } from 'sequelize';
import sequelize from './sequelize.js';

export class FinancesModels extends Model {}

FinancesModels.init({
    transaction_id: {
        type: DataTypes.INTEGER,
        primaryKey: true,
        autoIncrement: true
    },
    order_id: {
        type: DataTypes.INTEGER,
        allowNull: false
    },
    amount: {
        type: DataTypes.DECIMAL(10, 2),
        allowNull: false
    },
    type: {
        type: DataTypes.STRING,
        allowNull: false
    },
    transaction_date: {
        type: DataTypes.DATE,
        allowNull: false
    },
    payment_method: {
        type: DataTypes.STRING,
        allowNull: false
    },
    status: {
        type: DataTypes.STRING,
        allowNull: false
    }
}, {
    sequelize,
    modelName: 'Finances'
});

export default FinancesModels;