import ClothesModel from './ClothesModels.js';

async function syncDatabase() {
    try {
        await ClothesModel.sync({ force: true }); // This will create the table
        console.log('Database synchronized successfully.');

        // Add some sample rows
        await ClothesModel.bulkCreate([
            { name: 'Shirt', description: 'Casual shirt', size: 'M', pricePerHour: 2.55, state: 'New'  },
            { name: 'Dress', description: 'Summer dress', size: 'S', pricePerHour: 2.55, state: 'New' },
            { name: 'Jeans', description: 'Blue denim jeans', size: 'L', pricePerHour: 2.55, state: 'New' },
            { name: 'Jacket', description: 'Leather jacket', size: 'XL', pricePerHour: 2.55, state: 'New' },
        ]);

        console.log('Sample rows added to the table.');
    } catch (error) {
        console.error('Error synchronizing database:', error);
    } finally {
        // Close the Sequelize connection
        await ClothesModel.sequelize.close();
    }
}

syncDatabase();