import express from 'express'
import clothesRoutes from "./clothesRoutes.js"

const app = express();

app.use(express.json());
app.use(express.static('public')); // Serve static files from 'public' directory
app.use('/api/clothes', clothesRoutes);

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Server running on port ${PORT}`));