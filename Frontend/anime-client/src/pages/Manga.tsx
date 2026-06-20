import { useEffect, useState, useCallback } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import axios from "axios";
import api from "@/lib/api";
import { MangaCard, type Manga } from "@/components/manga/MangaCard";
import { Card } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { Star, BookOpen, ChevronLeft, ChevronRight, Search } from "lucide-react";
import { toast } from "sonner";

export function Manga() {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  const query = searchParams.get("q") ?? "";

  const [featured, setFeatured] = useState<Manga[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [nextIndex, setNextIndex] = useState<number | null>(null);
  const [isTransitioning, setIsTransitioning] = useState(false);
  const [isHeroLoading, setIsHeroLoading] = useState(true);

  const [searchResults, setSearchResults] = useState<Manga[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [searchInput, setSearchInput] = useState(query);

  useEffect(() => {
    const fetch = async () => {
      try {
        const response = await api.get<Manga[]>("/manga/top?page=1&pageSize=10");
        setFeatured(response.data);
        setCurrentIndex(Math.floor(Math.random() * response.data.length));
      } catch {
        // silently fail
      } finally {
        setIsHeroLoading(false);
      }
    };
    fetch();
  }, []);

  useEffect(() => {
    if (!query) return;
    const fetchResults = async () => {
      try {
        setIsSearching(true);
        const response = await api.get<Manga[]>(`/manga/search?query=${encodeURIComponent(query)}`);
        setSearchResults(response.data);
      } catch (err) {
        if (axios.isAxiosError(err)) toast.error("Search failed.");
      } finally {
        setIsSearching(false);
      }
    };
    fetchResults();
  }, [query]);

  const transitionTo = useCallback((index: number) => {
    if (isTransitioning || index === currentIndex) return;
    setNextIndex(index);
    setIsTransitioning(true);
    setTimeout(() => {
      setCurrentIndex(index);
      setNextIndex(null);
      setIsTransitioning(false);
    }, 700);
  }, [isTransitioning, currentIndex]);

  const goNext = useCallback(() => {
    if (featured.length === 0) return;
    transitionTo((currentIndex + 1) % featured.length);
  }, [featured.length, currentIndex, transitionTo]);

  const goPrev = useCallback(() => {
    if (featured.length === 0) return;
    transitionTo((currentIndex - 1 + featured.length) % featured.length);
  }, [featured.length, currentIndex, transitionTo]);

  useEffect(() => {
    if (featured.length === 0 || query) return;
    const timer = setInterval(goNext, 8000);
    return () => clearInterval(timer);
  }, [featured.length, goNext, query]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    const trimmed = searchInput.trim();
    if (!trimmed) return;
    setSearchParams({ q: trimmed });
  };

  const current = featured[currentIndex];
  const next = nextIndex !== null ? featured[nextIndex] : null;

  return (
    <div className="space-y-8">
      {!query && (
        <div className="relative w-full h-[85vh] overflow-hidden -mt-8">
          {isHeroLoading ? (
            <Skeleton className="w-full h-full" />
          ) : current ? (
            <>
              <div
                className="absolute inset-0 bg-cover bg-center scale-105 transition-opacity duration-700"
                style={{ backgroundImage: `url(${current.imageURL})`, opacity: isTransitioning ? 0 : 1 }}
              />
              {next && (
                <div
                  className="absolute inset-0 bg-cover bg-center scale-105 transition-opacity duration-700"
                  style={{ backgroundImage: `url(${next.imageURL})`, opacity: isTransitioning ? 1 : 0 }}
                />
              )}
              <div className="absolute inset-0 bg-black/60 backdrop-blur-sm" />
              <div className="absolute bottom-0 left-0 right-0 h-40 bg-gradient-to-t from-background to-transparent" />

              <div className="relative h-full flex items-center">
                <div className="container mx-auto px-8 max-w-screen-2xl flex items-center gap-12">
                  <div className="hidden lg:block flex-shrink-0 w-56 shadow-2xl rounded-lg overflow-hidden border border-white/10">
                    <img
                      src={current.imageURL}
                      alt={current.title}
                      className={`w-full h-full object-cover transition-opacity duration-700 ${isTransitioning ? "opacity-0" : "opacity-100"}`}
                    />
                  </div>

                  <div className={`flex flex-col gap-5 max-w-2xl transition-opacity duration-700 ${isTransitioning ? "opacity-0" : "opacity-100"}`}>
                    <div className="flex items-center gap-3">
                      <Badge variant="secondary" className="text-xs uppercase tracking-widest px-3 py-1">
                        Top Manga
                      </Badge>
                      {current.score > 0 && (
                        <div className="flex items-center gap-1.5 text-yellow-400 font-semibold text-sm">
                          <Star className="h-4 w-4 fill-yellow-400" />
                          {current.score.toFixed(1)}
                        </div>
                      )}
                    </div>

                    <h1 className="text-4xl md:text-6xl font-extrabold text-white leading-tight tracking-tight drop-shadow-lg">
                      {current.title}
                    </h1>

                    <div className="flex items-center gap-4 text-sm text-white/70 font-medium">
                      <span>{current.type || "Manga"}</span>
                      <span className="w-1 h-1 rounded-full bg-white/40" />
                      <span>{current.chapters > 0 ? `${current.chapters} Chapters` : "Ongoing"}</span>
                      <span className="w-1 h-1 rounded-full bg-white/40" />
                      <span>{current.status || "Publishing"}</span>
                    </div>

                    <div className="flex items-center gap-3 mt-2">
                      <Button size="lg" className="gap-2 font-semibold" onClick={() => navigate(`/manga/${current.mangaId}`)}>
                        <BookOpen className="h-5 w-5" /> View Details
                      </Button>
                    </div>
                  </div>
                </div>
              </div>

              <button onClick={goPrev} className="absolute left-4 top-1/2 -translate-y-1/2 p-2 rounded-full bg-black/40 text-white hover:bg-black/70 transition-colors">
                <ChevronLeft className="h-6 w-6" />
              </button>
              <button onClick={goNext} className="absolute right-4 top-1/2 -translate-y-1/2 p-2 rounded-full bg-black/40 text-white hover:bg-black/70 transition-colors">
                <ChevronRight className="h-6 w-6" />
              </button>

              <div className="absolute bottom-44 left-1/2 -translate-x-1/2 flex items-center gap-2">
                {featured.map((_, i) => (
                  <button
                    key={i}
                    onClick={() => transitionTo(i)}
                    className={`rounded-full transition-all duration-300 ${i === currentIndex ? "w-6 h-2 bg-white" : "w-2 h-2 bg-white/40 hover:bg-white/70"}`}
                  />
                ))}
              </div>
            </>
          ) : null}
        </div>
      )}

      {/* Search Bar */}
      <div className="container mx-auto px-4 max-w-screen-2xl">
        <form onSubmit={handleSearch} className="flex gap-2 max-w-lg">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground pointer-events-none" />
            <Input
              type="text"
              placeholder="Search manga..."
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
              className="pl-9"
            />
          </div>
          <Button type="submit">Search</Button>
        </form>
      </div>

      {/* Search Results */}
      {query && (
        <div className="container mx-auto px-4 max-w-screen-2xl space-y-4">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Search Results</h1>
            <p className="text-muted-foreground">Showing results for "{query}"</p>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
            {isSearching
              ? Array.from({ length: 10 }).map((_, i) => (
                  <Card key={i} className="overflow-hidden h-full flex flex-col">
                    <Skeleton className="aspect-[3/4] w-full" />
                    <div className="p-4 space-y-3 flex-grow">
                      <Skeleton className="h-5 w-3/4" />
                      <Skeleton className="h-4 w-1/2" />
                    </div>
                    <div className="p-4 pt-0">
                      <Skeleton className="h-9 w-full rounded-md" />
                    </div>
                  </Card>
                ))
              : searchResults.map((manga) => (
                  <MangaCard key={manga.mangaId} manga={manga} />
                ))}
          </div>

          {!isSearching && searchResults.length === 0 && (
            <div className="text-center py-12 text-muted-foreground">
              <p className="text-lg">No results found for "{query}".</p>
            </div>
          )}
        </div>
      )}
    </div>
  );
}