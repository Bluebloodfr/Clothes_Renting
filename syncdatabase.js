var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import sequelize from './sequelize.js';
import ClothesModel from './ClothesModels.js';
import AccountsModel from './AccountsModels.js';
import OrdersModel from './OrdersModels.js';
import FinancesModel from './FinancesModels.js';
OrdersModel.belongsTo(AccountsModel, { foreignKey: 'account_id' });
AccountsModel.hasMany(OrdersModel, { foreignKey: 'account_id' });
FinancesModel.belongsTo(OrdersModel, { foreignKey: 'order_id' });
OrdersModel.hasMany(FinancesModel, { foreignKey: 'order_id' });
OrdersModel.belongsTo(ClothesModel, { foreignKey: 'clothes_id' });
ClothesModel.hasMany(OrdersModel, { foreignKey: 'clothes_id' });
function syncDatabase() {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            yield sequelize.sync({ force: true });
            console.log('Database synchronized successfully.');
            // Add some sample rows
            const clothes = yield ClothesModel.bulkCreate([
                { name: 'Shirt', description: 'Casual shirt', size: 'M', pricePerHour: 2.55, state: 'New', imageURL: null },
                { name: 'Dress', description: 'Summer dress', size: 'S', pricePerHour: 2.55, state: 'New', imageURL: null },
                { name: 'Jeans', description: 'Blue denim jeans', size: 'L', pricePerHour: 2.55, state: 'New', imageURL: null },
                { name: 'Jacket', description: 'Leather jacket', size: 'XL', pricePerHour: 2.55, state: 'New', imageURL: null },
            ]);
            // Add some sample rows for AccountsModel
            const accounts = yield AccountsModel.bulkCreate([
                { username: 'user1', email: 'user1@example.com', password_hash: 'hash1', role: 'customer' },
                { username: 'user2', email: 'user2@example.com', password_hash: 'hash2', role: 'customer' },
                { username: 'admin', email: 'admin@example.com', password_hash: 'adminhash', role: 'admin' }
            ]);
            // Add some sample rows for OrdersModel
            const orders = yield OrdersModel.bulkCreate([
                { account_id: accounts[0].account_id, clothes_id: clothes[0].clothes_id, rental_start_date: new Date(), rental_end_date: new Date(), status: 'pending' },
                { account_id: accounts[1].account_id, clothes_id: clothes[1].clothes_id, rental_start_date: new Date(), rental_end_date: new Date(), status: 'rented' }
            ]);
            console.log('Sample rows added to the table.');
        }
        catch (error) {
            console.error('Error synchronizing database:', error);
        }
        finally {
            // Close the Sequelize connection
            yield ClothesModel.sequelize.close();
        }
    });
}
syncDatabase();
//# sourceMappingURL=syncdatabase.js.map