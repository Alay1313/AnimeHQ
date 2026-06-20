import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Toaster } from 'sonner';
import { Navbar } from '@/components/layout/Navbar';
import { Footer } from '@/components/layout/Footer';
import { Login } from '@/pages/Login';
import { Register } from '@/pages/Register';
import { Home } from '@/pages/Home';
import { AnimeDetails } from '@/pages/AnimeDetails';
import { TopAiring } from '@/pages/TopAiring';
import { Popular } from '@/pages/Popular';
import { Seasonal } from '@/pages/Seasonal';
import { Search } from '@/pages/Search';
import { Manga } from '@/pages/Manga';
import { MangaDetails } from '@/pages/MangaDetails';
import { MyList } from '@/pages/MyList';
import { Privacy } from '@/pages/Privacy';

function App() {
  return (
    <BrowserRouter>
      <div className="min-h-screen flex flex-col bg-background text-foreground">
        <Navbar />
        <main className="flex-1 container mx-auto px-4 py-8">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/anime/:id" element={<AnimeDetails />} />
            <Route path="/top-airing" element={<TopAiring />} />
            <Route path="/popular" element={<Popular />} />
            <Route path="/seasonal" element={<Seasonal />} />
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/search" element={<Search />} />
            <Route path="/manga" element={<Manga />} />
            <Route path="/manga/:id" element={<MangaDetails />} />
            <Route path="/my-list" element={<MyList />} />
            <Route path="/privacy" element={<Privacy />} />
          </Routes>
        </main>
        <Footer />
        <Toaster position="top-right" richColors />
      </div>
    </BrowserRouter>
  );
}

export default App;