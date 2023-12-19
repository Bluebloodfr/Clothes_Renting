import { Model, DataTypes } from 'sequelize';
import sequelize from './sequelize.js';

export class OrdersModels extends Model {}

OrdersModels.init({
    order_id: {
        type: DataTypes.INTEGER,
        primaryKey: true,
        autoIncrement: true
    },
    account_id: {
        type: DataTypes.INTEGER,
        allowNull: false
    },
    clothes_id: {
        type: DataTypes.INTEGER,
        allowNull: false
    },
    rental_start_date: {
        type: DataTypes.DATE,
        allowNull: false
    },
    rental_end_date: {
        type: DataTypes.DATE,
        allowNull: false
    },
    status: {
        type: DataTypes.STRING,
        allowNull: false
    },
}, {
    sequelize,
    modelName: 'Orders'
});

export default OrdersModels;