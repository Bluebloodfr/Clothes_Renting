import { Sequelize } from 'sequelize';

const database = 'ClothesRenting';
const username = 'ClothesRentingUser';
const password = 'abc';
const host = 'localhost';

// Creating a new Sequelize instance
const sequelize = new Sequelize(database, username, password, {
    host: host,
    dialect: 'postgres', 
    logging: false,
});

export default sequelize;