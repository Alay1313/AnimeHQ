import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import axios from "axios";
import api from "@/lib/api";
import { useAuthStore } from "@/store/authStore";
import { Skeleton } from "@/components/ui/skeleton";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/seperator";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Star, Tv, Film, Calendar, ArrowLeft, RefreshCw, Heart, List } from "lucide-react";
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

interface Review {
  reviewId: number;
  userId: string;
  animeId: number;
  content: string;
  rating: number;
  createdAt: string;
  userName: string | null;
  animeTitle: string | null;
}

interface Episode {
  id: number;
  animeId: number;
  title: string;
  synopsis: string;
  airDate: string;
  duration: string;
  episodeNumber: number;
}

function StarRating({ value, onChange }: { value: number; onChange: (v: number) => void }) {
  const [hovered, setHovered] = useState(0);
  return (
    <div className="flex gap-1">
      {Array.from({ length: 10 }, (_, i) => i + 1).map((star) => (
        <button
          key={star}
          type="button"
          onClick={() => onChange(star)}
          onMouseEnter={() => setHovered(star)}
          onMouseLeave={() => setHovered(0)}
          className="focus:outline-none"
        >
          <Star
            className={`h-6 w-6 transition-colors ${
              star <= (hovered || value)
                ? "fill-yellow-400 text-yellow-400"
                : "text-muted-foreground"
            }`}
          />
        </button>
      ))}
      {value > 0 && (
        <span className="ml-2 text-sm text-muted-foreground self-center">{value}/10</span>
      )}
    </div>
  );
}

export function AnimeDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user, isAuthenticated } = useAuthStore();

  const [anime, setAnime] = useState<AnimeDetails | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSyncing, setIsSyncing] = useState(false);
  const [isFavorite, setIsFavorite] = useState(false);
  const [isFavoriteLoading, setIsFavoriteLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  const [reviews, setReviews] = useState<Review[]>([]);
  const [reviewContent, setReviewContent] = useState("");
  const [reviewRating, setReviewRating] = useState(0);
  const [isSubmittingReview, setIsSubmittingReview] = useState(false);

  const [episodes, setEpisodes] = useState<Episode[]>([]);
  const [isSyncingEpisodes, setIsSyncingEpisodes] = useState(false);
  const [episodePage, setEpisodePage] = useState(1);
  const [hasMoreEpisodes, setHasMoreEpisodes] = useState(false);
  const EPISODES_PER_PAGE = 20;

  useEffect(() => {
    if (!id) return;
    let ignore = false;

    const fetchAnime = async () => {
      try {
        setError(null);
        const response = await api.get<AnimeDetails>(`/anime/${id}`);
        if (!ignore) {
          setAnime(response.data);
          setIsLoading(false);
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
    return () => { ignore = true; };
  }, [id, refreshKey]);

  // Fetch reviews
  useEffect(() => {
    if (!id) return;
    const fetchReviews = async () => {
      try {
        const response = await api.get<Review[]>(`/review/anime/${id}`);
        setReviews(response.data);
      } catch {
        // silently fail
      }
    };
    fetchReviews();
  }, [id]);

  // Fetch episodes
  useEffect(() => {
    if (!id) return;
    const fetchEpisodes = async () => {
      try {
        const response = await api.get<Episode[]>(
          `/episode/anime/${id}?page=1&pageSize=${EPISODES_PER_PAGE}`
        );
        setEpisodes(response.data);
        setHasMoreEpisodes(response.data.length === EPISODES_PER_PAGE);
        setEpisodePage(1);
      } catch {
        // silently fail
      }
    };
    fetchEpisodes();
  }, [id]);

  // Check if already favorited
  useEffect(() => {
    if (!id || !user || !isAuthenticated) return;
    const checkFavorite = async () => {
      try {
        const response = await api.get<{ isFavorite: boolean }>(
          `/favorite/check?userId=${user.id}&animeId=${id}`
        );
        setIsFavorite(response.data.isFavorite);
      } catch {
        // silently fail
      }
    };
    checkFavorite();
  }, [id, user, isAuthenticated]);

  const handleSync = async () => {
    if (!id) return;
    setIsSyncing(true);
    try {
      await api.post(`/anime/${id}/sync`);
      toast.success("Anime synced successfully from Jikan API!");
      setRefreshKey((prev) => prev + 1);
      setIsLoading(true);
    } catch {
      toast.error("Failed to sync anime. Please try again.");
    } finally {
      setIsSyncing(false);
    }
  };

  const handleSyncEpisodes = async () => {
    if (!id) return;
    setIsSyncingEpisodes(true);
    try {
      const response = await api.post<Episode[]>(`/episode/anime/${id}/sync`);
      setEpisodes(response.data.slice(0, EPISODES_PER_PAGE));
      setHasMoreEpisodes(response.data.length > EPISODES_PER_PAGE);
      setEpisodePage(1);
      toast.success(`Synced ${response.data.length} episodes from Jikan!`);
    } catch {
      toast.error("Failed to sync episodes. Please try again.");
    } finally {
      setIsSyncingEpisodes(false);
    }
  };

  const handleLoadMoreEpisodes = async () => {
    if (!id) return;
    const nextPage = episodePage + 1;
    try {
      const response = await api.get<Episode[]>(
        `/episode/anime/${id}?page=${nextPage}&pageSize=${EPISODES_PER_PAGE}`
      );
      setEpisodes((prev) => [...prev, ...response.data]);
      setHasMoreEpisodes(response.data.length === EPISODES_PER_PAGE);
      setEpisodePage(nextPage);
    } catch {
      toast.error("Failed to load more episodes.");
    }
  };

  const handleFavorite = async () => {
    if (!isAuthenticated || !user) {
      toast.error("Please sign in to add favorites.");
      navigate("/login");
      return;
    }
    if (!anime) return;
    setIsFavoriteLoading(true);
    try {
      if (isFavorite) {
        await api.delete(`/favorite?userId=${user.id}&animeId=${anime.animeListId}`);
        setIsFavorite(false);
        toast.success("Removed from favorites.");
      } else {
        await api.post("/favorite", { userId: user.id, animeId: anime.animeListId });
        setIsFavorite(true);
        toast.success("Added to favorites!");
      }
    } catch (err) {
      if (axios.isAxiosError(err)) {
        toast.error(err.response?.data || "Failed to update favorites.");
      } else {
        toast.error("Something went wrong.");
      }
    } finally {
      setIsFavoriteLoading(false);
    }
  };

  const handleSubmitReview = async () => {
    if (!isAuthenticated || !user) {
      toast.error("Please sign in to leave a review.");
      navigate("/login");
      return;
    }
    if (!anime) return;
    if (!reviewContent.trim()) {
      toast.error("Please write something in your review.");
      return;
    }
    if (reviewRating === 0) {
      toast.error("Please select a rating.");
      return;
    }
    setIsSubmittingReview(true);
    try {
      const response = await api.post<Review>("/review", {
        userId: user.id,
        animeId: anime.animeListId,
        content: reviewContent,
        rating: reviewRating,
      });
      setReviews((prev) => [response.data, ...prev]);
      setReviewContent("");
      setReviewRating(0);
      toast.success("Review submitted!");
    } catch (err) {
      if (axios.isAxiosError(err)) {
        toast.error(err.response?.data?.message || "Failed to submit review.");
      } else {
        toast.error("Something went wrong.");
      }
    } finally {
      setIsSubmittingReview(false);
    }
  };

  const handleDeleteReview = async (reviewId: number) => {
    try {
      await api.delete(`/review/${reviewId}`);
      setReviews((prev) => prev.filter((r) => r.reviewId !== reviewId));
      toast.success("Review deleted.");
    } catch {
      toast.error("Failed to delete review.");
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
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-8 space-y-4">
        <h2 className="text-2xl font-bold text-destructive">Anime Not Found</h2>
        <p className="text-muted-foreground">
          {error || "This anime isn't in the local database yet."}
        </p>
        <p className="text-sm text-muted-foreground">
          It may still be loading from Jikan — try syncing it directly.
        </p>
        <div className="flex gap-3">
          <Button onClick={() => navigate("/")} variant="outline">
            <ArrowLeft className="mr-2 h-4 w-4" /> Back to Home
          </Button>
          <Button
            onClick={async () => {
              if (!id) return;
              setIsLoading(true);
              setError(null);
              try {
                await api.post(`/anime/${id}/sync`);
                setRefreshKey((prev) => prev + 1);
              } catch {
                setError("Sync failed. Jikan may be rate-limiting. Try again in a moment.");
                setIsLoading(false);
              }
            }}
          >
            <RefreshCw className="mr-2 h-4 w-4" /> Sync from Jikan
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8 max-w-5xl space-y-8 animate-in fade-in duration-500">
      <Button variant="ghost" onClick={() => navigate(-1)} className="pl-0 hover:pl-2 transition-all">
        <ArrowLeft className="mr-2 h-4 w-4" /> Back
      </Button>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
        {/* Left Column: Poster */}
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
              {isSyncing
                ? <RefreshCw className="mr-2 h-4 w-4 animate-spin" />
                : <RefreshCw className="mr-2 h-4 w-4" />}
              {isSyncing ? "Syncing..." : "Sync from Jikan"}
            </Button>
            <Button
              variant={isFavorite ? "default" : "outline"}
              className={`w-full ${isFavorite ? "text-white" : ""}`}
              onClick={handleFavorite}
              disabled={isFavoriteLoading}
            >
              <Heart className={`mr-2 h-4 w-4 ${isFavorite ? "fill-current" : ""}`} />
              {isFavoriteLoading
                ? "Updating..."
                : isFavorite
                ? "Remove from Favorites"
                : "Add to Favorites"}
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
                <span>
                  <strong className="text-foreground">Aired:</strong>{" "}
                  {anime.airedFrom ? new Date(anime.airedFrom).getFullYear() : "?"} -{" "}
                  {anime.airedTo ? new Date(anime.airedTo).getFullYear() : "?"}
                </span>
              </div>
            </div>
          </div>

          <Separator />

          {anime.genreIds && anime.genreIds.length > 0 && (
            <div className="flex flex-wrap gap-2">
              {anime.genreIds.map((genreId) => (
                <Badge key={genreId} variant="outline">Genre {genreId}</Badge>
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

      {/* Episodes Section */}
      <Separator />
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <h2 className="text-2xl font-bold flex items-center gap-2">
            <List className="h-6 w-6" /> Episodes
          </h2>
          <Button
            variant="outline"
            size="sm"
            onClick={handleSyncEpisodes}
            disabled={isSyncingEpisodes}
          >
            {isSyncingEpisodes
              ? <RefreshCw className="mr-2 h-4 w-4 animate-spin" />
              : <RefreshCw className="mr-2 h-4 w-4" />}
            {isSyncingEpisodes ? "Syncing..." : "Sync Episodes"}
          </Button>
        </div>

        {episodes.length === 0 ? (
          <div className="text-center py-8 text-muted-foreground border rounded-lg bg-muted/50">
            <p>No episodes found.</p>
            <p className="text-sm mt-1">Click "Sync Episodes" to fetch them from Jikan.</p>
          </div>
        ) : (
          <div className="space-y-2">
            {episodes.map((episode, index) => (
              <div
                key={episode.id}
                className="flex items-start gap-4 p-4 border rounded-lg bg-card hover:bg-muted/50 transition-colors"
              >
                <div className="flex-shrink-0 w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center text-primary font-bold text-sm">
                  {index + 1}
                </div>
                <div className="flex-1 min-w-0">
                  <p className="font-semibold truncate">{episode.title || `Episode ${index + 1}`}</p>
                  <div className="flex items-center gap-3 text-xs text-muted-foreground mt-1">
                    {episode.airDate && (
                      <span>{new Date(episode.airDate).toLocaleDateString()}</span>
                    )}
                    {episode.duration && (
                      <span>{episode.duration}</span>
                    )}
                  </div>
                </div>
              </div>
            ))}

            {hasMoreEpisodes && (
              <Button
                variant="outline"
                className="w-full mt-4"
                onClick={handleLoadMoreEpisodes}
              >
                Load More Episodes
              </Button>
            )}
          </div>
        )}
      </div>

      {/* Reviews Section */}
      <Separator />
      <div className="space-y-6">
        <h2 className="text-2xl font-bold">Reviews</h2>

        {isAuthenticated ? (
          <div className="border rounded-lg p-6 space-y-4 bg-card">
            <h3 className="text-lg font-semibold">Write a Review</h3>
            <StarRating value={reviewRating} onChange={setReviewRating} />
            <Textarea
              placeholder="Share your thoughts about this anime..."
              value={reviewContent}
              onChange={(e) => setReviewContent(e.target.value)}
              rows={4}
              className="resize-none"
            />
            <Button onClick={handleSubmitReview} disabled={isSubmittingReview}>
              {isSubmittingReview ? "Submitting..." : "Submit Review"}
            </Button>
          </div>
        ) : (
          <div className="border rounded-lg p-6 text-center text-muted-foreground bg-muted/50">
            <p>
              <button onClick={() => navigate("/login")} className="text-primary underline">
                Sign in
              </button>{" "}
              to leave a review.
            </p>
          </div>
        )}

        {reviews.length === 0 ? (
          <p className="text-muted-foreground text-center py-8">
            No reviews yet. Be the first to review this anime!
          </p>
        ) : (
          <div className="space-y-4">
            {reviews.map((review) => (
              <div key={review.reviewId} className="border rounded-lg p-5 space-y-3 bg-card">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <div className="h-8 w-8 rounded-full bg-primary flex items-center justify-center text-primary-foreground font-semibold text-sm">
                      {review.userName?.charAt(0).toUpperCase() ?? "?"}
                    </div>
                    <div>
                      <p className="font-semibold text-sm">{review.userName ?? "Anonymous"}</p>
                      <p className="text-xs text-muted-foreground">
                        {new Date(review.createdAt).toLocaleDateString()}
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <div className="flex items-center gap-1 text-yellow-400 font-semibold text-sm">
                      <Star className="h-4 w-4 fill-yellow-400" />
                      {review.rating}/10
                    </div>
                    {user && review.userId === user.id && (
                      <Button
                        variant="ghost"
                        size="sm"
                        className="text-destructive hover:text-destructive"
                        onClick={() => handleDeleteReview(review.reviewId)}
                      >
                        Delete
                      </Button>
                    )}
                  </div>
                </div>
                <p className="text-muted-foreground leading-relaxed">{review.content}</p>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}