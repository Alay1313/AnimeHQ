document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('searchInput');
    const searchBtn = document.getElementById('searchBtn');
    const resultsContainer = document.getElementById('resultsContainer');

    async function handleSearch() {
        const query = searchInput.value.trim();
        if (!query) return;

        
        resultsContainer.innerHTML = '<p class="loading">Searching...</p>';

        try {
            
            const response = await fetch(
           `http://localhost:5258/api/anime/search?query=${encodeURIComponent(query)}`
        );
            
            if (!response.ok) {
                throw new Error(`Search failed: ${response.status}`);
            }

            const animes = await response.json();
            resultsContainer.innerHTML = ''; 

            
            if (!animes || animes.length === 0) {
                resultsContainer.innerHTML = '<p class="no-results">No anime found matching your search.</p>';
                return;
            }

            
            animes.forEach(anime => {
                const card = document.createElement('div');
                card.className = 'anime-card';
                card.innerHTML = `
                    <img src="${anime.imageURL || '/images/placeholder.jpg'}" alt="${anime.title}" onerror="this.src='/images/placeholder.jpg'">
                    <h3>${anime.title}</h3>
                    <p class="score">⭐ ${anime.score ?? 'N/A'}</p>
                    <p class="type">${anime.type || 'Unknown'}</p>
                `;
                resultsContainer.appendChild(card);
            });

        } catch (error) {
            console.error('Search error:', error);
            resultsContainer.innerHTML = '<p class="error">Failed to load results. Please try again.</p>';
        }
    }

    
    searchBtn.addEventListener('click', handleSearch);
    searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') handleSearch();
    });
    
});