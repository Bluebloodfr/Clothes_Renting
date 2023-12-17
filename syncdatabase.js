var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import ClothesModel from './ClothesModels.js';
function syncDatabase() {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            yield ClothesModel.sync({ force: true }); // This will create the table
            console.log('Database synchronized successfully.');
            // Add some sample rows
            yield ClothesModel.bulkCreate([
                { name: 'Shirt', description: 'Casual shirt', size: 'M', pricePerHour: 2.55, state: 'New' },
                { name: 'Dress', description: 'Summer dress', size: 'S', pricePerHour: 2.55, state: 'New' },
                { name: 'Jeans', description: 'Blue denim jeans', size: 'L', pricePerHour: 2.55, state: 'New' },
                { name: 'Jacket', description: 'Leather jacket', size: 'XL', pricePerHour: 2.55, state: 'New' },
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