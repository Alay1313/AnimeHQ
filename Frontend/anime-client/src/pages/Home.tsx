import { useEffect, useState, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import api from "@/lib/api";
import { type Anime } from "@/components/anime/AnimeCard";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { Star, Play, Info, ChevronLeft, ChevronRight, RefreshCw } from "lucide-react";

export function Home() {
  const [animes, setAnimes] = useState<Anime[]>([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [nextIndex, setNextIndex] = useState<number | null>(null);
  const [isTransitioning, setIsTransitioning] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(false);
  const [retryKey, setRetryKey] = useState(0);
  const [sourceLabel, setSourceLabel] = useState("Now Airing");
  const navigate = useNavigate();

  const handleRetry = () => setRetryKey((k) => k + 1);

  useEffect(() => {
    let ignore = false;

    const fetchAnimes = async () => {
      setIsLoading(true);
      setError(false);

      // Try top-airing first, fall back to seasonal if it 504s
      const endpoints: { url: string; label: string }[] = [
        { url: "/anime/top-airing?page=1&pageSize=10", label: "Now Airing" },
        { url: "/anime/seasonal?page=1&pageSize=10",   label: "This Season" },
        { url: "/anime/popular?page=1&pageSize=10",    label: "Popular"     },
      ];

      for (const { url, label } of endpoints) {
        try {
          const response = await api.get<Anime[]>(url);
          if (!ignore && response.data.length > 0) {
            setAnimes(response.data);
            setSourceLabel(label);
            setCurrentIndex(Math.floor(Math.random() * response.data.length));
            setIsLoading(false);
            return; // success — stop trying
          }
        } catch {
          // this endpoint failed, try the next one
        }
      }

      // all endpoints failed
      if (!ignore) {
        setError(true);
        setIsLoading(false);
      }
    };

    fetchAnimes();
    return () => { ignore = true; };
  }, [retryKey]);

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
    if (animes.length === 0) return;
    transitionTo((currentIndex + 1) % animes.length);
  }, [animes.length, currentIndex, transitionTo]);

  const goPrev = useCallback(() => {
    if (animes.length === 0) return;
    transitionTo((currentIndex - 1 + animes.length) % animes.length);
  }, [animes.length, currentIndex, transitionTo]);

  useEffect(() => {
    if (animes.length === 0) return;
    const timer = setInterval(goNext, 8000);
    return () => clearInterval(timer);
  }, [animes.length, goNext]);

  if (isLoading) {
    return <Skeleton className="w-full h-[85vh]" />;
  }

  if (error || animes.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center h-[85vh] text-center space-y-4">
        <h2 className="text-2xl font-bold">Jikan API is temporarily unavailable</h2>
        <p className="text-muted-foreground">Please try again in a few minutes.</p>
        <Button onClick={handleRetry} variant="outline">
          <RefreshCw className="mr-2 h-4 w-4" /> Retry
        </Button>
      </div>
    );
  }

  const featured = animes[currentIndex];
  const next = nextIndex !== null ? animes[nextIndex] : null;

  return (
    <div className="relative w-full h-[85vh] overflow-hidden">
      <div
        className="absolute inset-0 bg-cover bg-center scale-105 transition-opacity duration-700"
        style={{ backgroundImage: `url(${featured.imageURL})`, opacity: isTransitioning ? 0 : 1 }}
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
              src={featured.imageURL}
              alt={featured.title}
              className={`w-full h-full object-cover transition-opacity duration-700 ${isTransitioning ? "opacity-0" : "opacity-100"}`}
            />
          </div>

          <div className={`flex flex-col gap-5 max-w-2xl transition-opacity duration-700 ${isTransitioning ? "opacity-0" : "opacity-100"}`}>
            <div className="flex items-center gap-3">
              <Badge variant="secondary" className="text-xs uppercase tracking-widest px-3 py-1">
                {sourceLabel}
              </Badge>
              {featured.score > 0 && (
                <div className="flex items-center gap-1.5 text-yellow-400 font-semibold text-sm">
                  <Star className="h-4 w-4 fill-yellow-400" />
                  {featured.score.toFixed(1)}
                </div>
              )}
            </div>

            <h1 className="text-4xl md:text-6xl font-extrabold text-white leading-tight tracking-tight drop-shadow-lg">
              {featured.title}
            </h1>

            <div className="flex items-center gap-4 text-sm text-white/70 font-medium">
              <span>{featured.type || "TV"}</span>
              <span className="w-1 h-1 rounded-full bg-white/40" />
              <span>{featured.episodes > 0 ? `${featured.episodes} Episodes` : "Ongoing"}</span>
              <span className="w-1 h-1 rounded-full bg-white/40" />
              <span>{featured.status || "Airing"}</span>
            </div>

            <div className="flex items-center gap-3 mt-6">
              <Button
                size="lg"
                className="gap-2 font-semibold"
                onClick={() => navigate(`/anime/${featured.animeListId}`)}
              >
                <Play className="h-5 w-5 fill-current" /> Watch Now
              </Button>
              <Button
                size="lg"
                variant="outline"
                className="gap-2 font-semibold bg-white/10 border-white/20 text-white hover:bg-white/20 hover:text-white"
                onClick={() => navigate(`/anime/${featured.animeListId}`)}
              >
                <Info className="h-5 w-5" /> More Info
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

      <div className="absolute bottom-64 left-1/2 -translate-x-1/2 flex items-center gap-4">
        {animes.map((_, i) => (
          <button
            key={i}
            onClick={() => transitionTo(i)}
            className={`rounded-full transition-all duration-300 ${
              i === currentIndex ? "w-6 h-2 bg-white" : "w-2 h-2 bg-white/40 hover:bg-white/70"
            }`}
          />
        ))}
      </div>
    </div>
  );
}