import { Model, DataTypes } from 'sequelize';
import sequelize from './sequelize.js';

export class AccountsModels extends Model {}

AccountsModels.init({
        account_id: {
            type: DataTypes.INTEGER,
            primaryKey: true,
            autoIncrement: true
        },
        username: {
            type: DataTypes.STRING,
            allowNull: false
        },
        email: {
            type: DataTypes.STRING,
            allowNull: false,
            unique: true
        },
        password_hash: {
            type: DataTypes.STRING,
            allowNull: false
        },
        role: {
            type: DataTypes.STRING,
            allowNull: false
        },
}, {
    sequelize,
    modelName: 'Accounts'
});

export default AccountsModels;