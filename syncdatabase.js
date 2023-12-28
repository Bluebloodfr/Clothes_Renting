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
import PaymentDetails from './PaymentDetails.js';
import bcrypt from 'bcrypt';
OrdersModel.belongsTo(AccountsModel, { foreignKey: 'account_id' });
AccountsModel.hasMany(OrdersModel, { foreignKey: 'account_id' });
PaymentDetails.belongsTo(AccountsModel, { foreignKey: 'account_id' });
AccountsModel.hasMany(PaymentDetails, { foreignKey: 'account_id' });
PaymentDetails.belongsTo(OrdersModel, { foreignKey: 'order_id' });
OrdersModel.hasMany(PaymentDetails, { foreignKey: 'order_id' });
ClothesModel.belongsTo(OrdersModel, { foreignKey: 'order_id' });
OrdersModel.hasMany(ClothesModel, { foreignKey: 'order_id' });
function syncDatabase() {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            yield sequelize.sync({ force: true });
            console.log('Database synchronized successfully.');
            const adminpassword = yield bcrypt.hash('adminhash', 10);
            const accounts = yield AccountsModel.bulkCreate([
                { username: 'user1', email: 'user1@example.com', password_hash: 'hash1', role: 'customer' },
                { username: 'user2', email: 'user2@example.com', password_hash: 'hash2', role: 'customer' },
                { username: 'admin', email: 'admin@example.com', password_hash: adminpassword, role: 'admin' }
            ]);
            const orders = yield OrdersModel.bulkCreate([
                { account_id: accounts[0].account_id, rental_start_date: new Date(), rental_end_date: new Date(), status: 'pending' },
                { account_id: accounts[1].account_id, rental_start_date: new Date(), rental_end_date: new Date(), status: 'rented' }
            ]);
            const clothes = yield ClothesModel.bulkCreate([
                { name: 'Shirt', order_id: orders[0].order_id, description: 'Casual shirt', size: 'M', pricePerDay: 2.55, state: 'New', imageURL: null },
                { name: 'Dress', order_id: orders[1].order_id, description: 'Summer dress', size: 'S', pricePerDay: 2.55, state: 'New', imageURL: null },
                { name: 'Jeans', description: 'Blue denim jeans', size: 'L', pricePerDay: 2.55, state: 'New', imageURL: null },
                { name: 'Jacket', description: 'Leather jacket', size: 'XL', pricePerDay: 2.55, state: 'New', imageURL: null },
            ]);
            console.log('Sample rows added to the table.');
        }
        catch (error) {
            console.error('Error synchronizing database:', error);
        }
        finally {
            yield ClothesModel.sequelize.close();
        }
    });
}
syncDatabase();
//# sourceMappingURL=syncdatabase.js.map