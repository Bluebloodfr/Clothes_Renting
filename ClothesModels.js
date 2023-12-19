import { Model, DataTypes } from 'sequelize';
import sequelize from './sequelize.js';

export class ClothesModel extends Model {}

ClothesModel.init({
    clothes_id: {
        type: DataTypes.INTEGER,
        autoIncrement: true,
        primaryKey: true,
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
    pricePerHour: {
        type: DataTypes.DECIMAL(10, 2), // Adjust the precision as needed
        allowNull: false,
    },
    state: {
        type: DataTypes.ENUM('New', 'Used', 'Deteriorating'), // Define the possible states
        allowNull: false,
    },
    imageURL: {
        type: DataTypes.STRING,
        allowNull: true,
    },
}, {
    sequelize,
    modelName: 'Clothes'
});

export default ClothesModel;