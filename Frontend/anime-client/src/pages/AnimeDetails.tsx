import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import axios from "axios";
import api from "@/lib/api";
import { Skeleton } from "@/components/ui/skeleton";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/seperator";
import { Button } from "@/components/ui/button";
import { Star, Tv, Film, Calendar, ArrowLeft, RefreshCw, Heart } from "lucide-react";
import { toast } from "sonner";

interface AnimeDetails {
  animeListId: number;
  title: string;
  imageURL: string;
  score: number;
  synopsis: string;
  type: string;
  episodes: number;
  status: string;
  airedFrom: string;
  airedTo: string;
  genreIds: number[];
}

export function AnimeDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  
  const [anime, setAnime] = useState<AnimeDetails | null>(null);
  // Initialize to true so we don't need to call setIsLoading(true) synchronously in useEffect
  const [isLoading, setIsLoading] = useState(true); 
  const [isSyncing, setIsSyncing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  //Using a refreshKey to trigger re-fetches cleanly
  const [refreshKey, setRefreshKey] = useState(0);

  useEffect(() => {
    if (!id) return;
    let ignore = false;
    
    const fetchAnime = async () => {
      try {
        setError(null);
        const response = await api.get<AnimeDetails>(`/anime/${id}`);
        if (!ignore) {
          setAnime(response.data);
          setIsLoading(false); // Only called AFTER the await, preventing the cascading render warning
        }
      } catch (error: unknown) {
        if (!ignore) {
          if (axios.isAxiosError(error)) {
            setError(error.response?.data?.message || "Failed to fetch anime details.");
          } else {
            setError("An unexpected error occurred.");
          }
          setIsLoading(false);
        }
      }
    };

    fetchAnime();

    return () => {
      ignore = true; // Cleanup to prevent state updates on unmounted components
    };
  }, [id, refreshKey]);

  const handleSync = async () => {
    if (!id) return;
    setIsSyncing(true);
    try {
      await api.post(`/anime/${id}/sync`);
      toast.success("Anime synced successfully from Jikan API!");
      
      
      setRefreshKey((prev) => prev + 1); 
      setIsLoading(true); // Show loading state again (this is AFTER an await, so it's safe)
    } catch { 
      
      toast.error("Failed to sync anime. Please try again.");
    } finally {
      setIsSyncing(false);
    }
  };

  if (isLoading) {
    return (
      <div className="container mx-auto px-4 py-8 max-w-5xl space-y-8">
        <Skeleton className="h-10 w-32" />
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          <Skeleton className="aspect-[3/4] w-full rounded-lg" />
          <div className="md:col-span-2 space-y-4">
            <Skeleton className="h-12 w-3/4" />
            <Skeleton className="h-6 w-1/4" />
            <Skeleton className="h-32 w-full" />
          </div>
        </div>
      </div>
    );
  }

  if (error || !anime) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-8">
        <h2 className="text-2xl font-bold text-destructive mb-2">Anime Not Found</h2>
        <p className="text-muted-foreground mb-6">{error || "The requested anime could not be found."}</p>
        <Button onClick={() => navigate("/")} variant="outline">
          <ArrowLeft className="mr-2 h-4 w-4" /> Back to Home
        </Button>
        <Button onClick={() => setRefreshKey((prev) => prev + 1)} className="mt-4">
          <RefreshCw className="mr-2 h-4 w-4" /> Retry
        </Button>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8 max-w-5xl space-y-8 animate-in fade-in duration-500">
      <Button variant="ghost" onClick={() => navigate(-1)} className="pl-0 hover:pl-2 transition-all">
        <ArrowLeft className="mr-2 h-4 w-4" /> Back
      </Button>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
        {/* Left Column: Poster Image */}
        <div className="md:col-span-1">
          <div className="relative aspect-[3/4] overflow-hidden rounded-lg border bg-muted shadow-lg">
            <img 
              src={anime.imageURL || "https://via.placeholder.com/400x600?text=No+Image"} 
              alt={anime.title}
              className="h-full w-full object-cover"
            />
          </div>
          
          <div className="mt-4 space-y-3">
            <Button className="w-full" onClick={handleSync} disabled={isSyncing}>
              {isSyncing ? (
                <RefreshCw className="mr-2 h-4 w-4 animate-spin" />
              ) : (
                <RefreshCw className="mr-2 h-4 w-4" />
              )}
              {isSyncing ? "Syncing..." : "Sync from Jikan"}
            </Button>
            <Button variant="outline" className="w-full">
              <Heart className="mr-2 h-4 w-4" /> Add to Favorites
            </Button>
          </div>
        </div>

        {/* Right Column: Details */}
        <div className="md:col-span-2 space-y-6">
          <div>
            <h1 className="text-3xl md:text-4xl font-bold tracking-tight mb-2">{anime.title}</h1>
            
            {anime.score > 0 && (
              <Badge variant="secondary" className="text-base px-3 py-1 mb-4">
                <Star className="mr-1.5 h-4 w-4 fill-yellow-400 text-yellow-400" />
                {anime.score.toFixed(1)} / 10
              </Badge>
            )}

            <div className="grid grid-cols-2 gap-4 text-sm">
              <div className="flex items-center gap-2 text-muted-foreground">
                <Tv className="h-4 w-4 text-primary" />
                <span><strong className="text-foreground">Type:</strong> {anime.type || "Unknown"}</span>
              </div>
              <div className="flex items-center gap-2 text-muted-foreground">
                <Film className="h-4 w-4 text-primary" />
                <span><strong className="text-foreground">Episodes:</strong> {anime.episodes > 0 ? anime.episodes : "?"}</span>
              </div>
              <div className="flex items-center gap-2 text-muted-foreground">
                <Calendar className="h-4 w-4 text-primary" />
                <span><strong className="text-foreground">Status:</strong> {anime.status || "Unknown"}</span>
              </div>
              <div className="flex items-center gap-2 text-muted-foreground">
                <Calendar className="h-4 w-4 text-primary" />
                <span><strong className="text-foreground">Aired:</strong> {anime.airedFrom ? new Date(anime.airedFrom).getFullYear() : "?"} - {anime.airedTo ? new Date(anime.airedTo).getFullYear() : "?"}</span>
              </div>
            </div>
          </div>

          <Separator />

          {anime.genreIds && anime.genreIds.length > 0 && (
            <div className="flex flex-wrap gap-2">
              {anime.genreIds.map((genreId) => (
                <Badge key={genreId} variant="outline">
                  Genre {genreId}
                </Badge>
              ))}
            </div>
          )}

          <div>
            <h2 className="text-xl font-semibold mb-3">Synopsis</h2>
            <p className="text-muted-foreground leading-relaxed whitespace-pre-line">
              {anime.synopsis || "No synopsis available for this anime."}
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}