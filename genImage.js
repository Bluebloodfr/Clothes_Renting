import axios from 'axios';

async function generateImageURL(description) {
    try {
        const response = await axios.post('https://api.openai.com/v1/images', {
            model: "image-alpha-001",
            description: description,
        }, {
            headers: {
                'Authorization': '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',
                'Content-Type': 'application/json',
            },
        });

        if (response.data && response.data.status === 'completed') {
            const imageURL = response.data.url;
            return imageURL;
        } else {
            console.error('Error generating image:', response.data);
            return null;
        }
    } catch (error) {
        console.error('Error generating image:', error);
        return null;
    }
}

export { generateImageURL };


