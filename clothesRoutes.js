import express from 'express';
import ClothesModel from './ClothesModels.js'; // Ensure the path is correct

const router = express.Router();

router.post('/add', async (req, res) => {
    try {
        const { name, description, size } = req.body;
        const newClothingItem = await ClothesModel.create({ name, description, size });
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

router.delete('/delete/:id', async (req, res) => {
    try {
        const id = req.params.id;
        await ClothesModel.destroy({ where: { id } });
        res.send('Item deleted successfully');
    } catch (error) {
        console.error(error);
        res.status(500).send('Error deleting item');
    }
});

router.put('/update/:id', async (req, res) => {
    try {
        const id = req.params.id;
        const { pricePerHour, state } = req.body;
        await ClothesModel.update({ pricePerHour, state }, { where: { id } });
        res.send('Item updated successfully');
    } catch (error) {
        console.error(error);
        res.status(500).send('Error updating item');
    }
});

export default router;