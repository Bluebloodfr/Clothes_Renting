import { Model, DataTypes } from 'sequelize';
import sequelize from './sequelize.js';

export class ClothesModel extends Model {}

ClothesModel.init({
    clothes_id: {
        type: DataTypes.INTEGER,
        autoIncrement: true,
        primaryKey: true,
    },
    order_id: {
        type: DataTypes.INTEGER,
        allowNull: true,
    },
    name: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    description: {
        type: DataTypes.TEXT,
        allowNull: false,
    },
    size: {
        type: DataTypes.STRING,
        allowNull: false,
    },
    pricePerDay: {
        type: DataTypes.DECIMAL(10, 2),
        allowNull: false,
    },
    state: {
        type: DataTypes.ENUM('New', 'Used', 'Deteriorating'),
        allowNull: false,
    },
    imageURL: {
        type: DataTypes.STRING,
        allowNull: true,
    },
}, {
    sequelize,
    modelName: 'Clothes',
    tableName: 'Clothes'
});

export default ClothesModel;