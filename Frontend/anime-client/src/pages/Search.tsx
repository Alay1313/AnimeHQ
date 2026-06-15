import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import axios from "axios";
import api from "@/lib/api";
import { AnimeCard, type Anime } from "@/components/anime/AnimeCard";
import { Card } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { toast } from "sonner";

export function Search() {
  const [searchParams] = useSearchParams();
  const query = searchParams.get("q") ?? "";

  const [animes, setAnimes] = useState<Anime[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!query) return;

    const fetchResults = async () => {
      try {
        setIsLoading(true);
        setError(null);
        const response = await api.get<Anime[]>(`/anime/search?query=${encodeURIComponent(query)}`);
        setAnimes(response.data);
      } catch (err: unknown) {
        if (axios.isAxiosError(err)) {
          setError(err.response?.data?.message || "Search failed.");
        } else {
          setError("An unexpected error occurred.");
        }
        toast.error("Search failed");
      } finally {
        setIsLoading(false);
      }
    };

    fetchResults();
  }, [query]);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Search Results</h1>
        <p className="text-muted-foreground">
          {query ? `Showing results for "${query}"` : "Enter a search term above."}
        </p>
      </div>

      {error && (
        <div className="text-center py-12 text-destructive">
          <p>{error}</p>
        </div>
      )}

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

      {!isLoading && !error && animes.length === 0 && query && (
        <div className="text-center py-12 text-muted-foreground">
          <p className="text-lg">No results found for "{query}".</p>
        </div>
      )}
    </div>
  );
}