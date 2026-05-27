const grid = document.getElementById('animeGrid');


const API_BASE = 'http://localhost:5000/api'; 
const ENDPOINT = `${API_BASE}/anime/top-airing`;

function renderAnime(animeList) {
  grid.innerHTML = '';
  
  if (!animeList?.length) {
    grid.innerHTML = '<div class="loading">No airing anime found.</div>';
    return;
  }

  animeList.forEach(anime => {
    const card = document.createElement('div');
    card.className = 'card';
    card.innerHTML = `
      <img src="${anime.imageURL || anime.image}" 
           alt="${anime.title}" 
           loading="lazy" 
           onerror="this.src='https://via.placeholder.com/240x340/16161e/9a9aa5?text=No+Image'">
      <div class="card-overlay">★ ${anime.score?.toFixed(1) || 'N/A'}</div>
      <div class="card-info">
        <div class="card-title" title="${anime.title}">${anime.title}</div>
        <div class="meta-row">
          <span>📺 ${anime.episodes || '?'} eps</span>
          <span class="status-badge">${anime.status || 'Airing'}</span>
        </div>
        <div class="genres">${anime.genres || anime.genreIds?.join(', ') || 'N/A'}</div>
      </div>
    `;
    grid.appendChild(card);
  });
}

async function fetchTopAiring() {
  try {
    grid.innerHTML = '<div class="loading">Loading top airing anime...</div>';
    
    const res = await fetch(ENDPOINT, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
      
    });
    
    if (!res.ok) {
      throw new Error(`HTTP error! status: ${res.status}`);
    }
    
    const data = await res.json();
    renderAnime(data);
    
  } catch (err) {
    console.error('Failed to fetch top airing anime:', err);
    
    
    if (location.hostname === 'localhost' || location.hostname === '127.0.0.1') {
      console.warn('⚠️ Falling back to mock data for development');
      renderAnime(mockData);
    } else {
      grid.innerHTML = '<div class="error">Failed to load data. Please try again later.</div>';
    }
  }
}


const mockData = [
  { title: "Solo Leveling", imageURL: "https://cdn.myanimelist.net/images/anime/1908/135313.jpg", score: 8.7, episodes: 13, status: "Airing", genres: "Action, Fantasy" },
  { title: "Kaiju No. 8", imageURL: "https://cdn.myanimelist.net/images/anime/1519/137063.jpg", score: 8.3, episodes: 12, status: "Airing", genres: "Action, Sci-Fi" }
];

document.addEventListener('DOMContentLoaded', fetchTopAiring);