import { useEffect, useState } from "react";
import axios from "axios";
import api from "@/lib/api";
import { AnimeCard, type Anime } from "@/components/anime/AnimeCard";
import { Card } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { RefreshCw } from "lucide-react";
import { toast } from "sonner";

interface AnimeListPageProps {
  title: string;
  description: string;
  endpoint: string;
}

export function AnimeListPage({ title, description, endpoint }: AnimeListPageProps) {
  const [animes, setAnimes] = useState<Anime[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [retryKey, setRetryKey] = useState(0);

  const handleRetry = () => setRetryKey((k) => k + 1);

  useEffect(() => {
    let ignore = false;

    const fetchAnimes = async () => {
      try {
        setIsLoading(true);
        setError(null);
        const response = await api.get<Anime[]>(endpoint);
        const unique = response.data.filter((anime, index, self) => index === self.findIndex((a) => a.animeListId === anime.animeListId));
        if (!ignore) setAnimes(unique);
      } catch (err: unknown) {
        if (!ignore) {
          if (axios.isAxiosError(err)) {
            setError(err.response?.data?.message || "Failed to fetch anime.");
          } else {
            setError("An unexpected error occurred.");
          }
          toast.error(`Failed to load ${title}`);
        }
      } finally {
        if (!ignore) setIsLoading(false);
      }
    };

    fetchAnimes();
    return () => { ignore = true; };
  }, [endpoint, retryKey]);

  if (error) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-8 border rounded-lg bg-muted/50 space-y-4">
        <h2 className="text-2xl font-bold text-destructive">Something went wrong.</h2>
        <p className="text-muted-foreground">{error}</p>
        <Button onClick={handleRetry} variant="outline">
          <RefreshCw className="mr-2 h-4 w-4" /> Retry
        </Button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">{title}</h1>
        <p className="text-muted-foreground">{description}</p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
        {isLoading
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
          : animes.map((anime) => (
              <AnimeCard key={anime.animeListId} anime={anime} />
            ))}
      </div>

      {!isLoading && animes.length === 0 && (
        <div className="flex flex-col items-center justify-center py-16 text-center space-y-4">
          <p className="text-lg text-muted-foreground">
            Jikan API is temporarily unavailable. Please try again in a few minutes.
          </p>
          <Button onClick={handleRetry} variant="outline">
            <RefreshCw className="mr-2 h-4 w-4" /> Retry
          </Button>
        </div>
      )}
    </div>
  );
}