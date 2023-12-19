import express from 'express';
import ClothesModel from './ClothesModels.js';
import { generateImageURL } from './genImage.js';

const router = express.Router();

router.post('/add', async (req, res) => {
    try {
        const { name, description, size, pricePerHour, state } = req.body;
        const newClothingItem = await ClothesModel.create({ name, description, size, pricePerHour, state, imageURL: null });
        res.status(201).json(newClothingItem);
    } catch (error) {
        console.error(error);
        res.status(500).send('Error adding new clothing item');
    }
});

router.get('/', async (req, res) => {
    try {
        const items = await ClothesModel.findAll();
        res.json(items);
    } catch (error) {
        console.error(error);
        res.status(500).send('Error retrieving items');
    }
});

router.delete('/delete/:clothes_id', async (req, res) => {
    try {
        const clothes_id = req.params.clothes_id;
        await ClothesModel.destroy({ where: { clothes_id } });
        res.send('Item deleted successfully');
    } catch (error) {
        console.error(error);
        res.status(500).send('Error deleting item');
    }
});

router.put('/update/:clothes_id', async (req, res) => {
    try {
        const clothes_id = req.params.clothes_id;
        const { pricePerHour, state } = req.body;
        await ClothesModel.update({ pricePerHour, state }, { where: { clothes_id } });
        res.send('Item updated successfully');
    } catch (error) {
        console.error(error);
        res.status(500).send('Error updating item');
    }
});

export default router;