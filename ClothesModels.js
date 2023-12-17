import { Model, DataTypes } from 'sequelize';
import sequelize from './sequelize.js'; // Adjust the path to your Sequelize config

export class ClothesModel extends Model {}

ClothesModel.init({
    id: {
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
}, {
    sequelize,
    modelName: 'Clothes'
});

export default ClothesModel;