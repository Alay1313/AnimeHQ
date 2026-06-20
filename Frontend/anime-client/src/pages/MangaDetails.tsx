import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import axios from "axios";
import api from "@/lib/api";
import { useAuthStore } from "@/store/authStore";
import { Skeleton } from "@/components/ui/skeleton";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Separator } from "@/components/ui/seperator";
import { Star, BookOpen, ArrowLeft, RefreshCw, Heart } from "lucide-react";
import { toast } from "sonner";

interface MangaDetails {
  mangaId: number;
  title: string;
  imageURL: string;
  score: number;
  synopsis: string;
  type: string;
  chapters: number;
  volumes: number;
  status: string;
  publishedFrom: string;
  publishedTo: string;
}

interface MangaReview {
  reviewId: number;
  userId: string;
  mangaId: number;
  content: string;
  rating: number;
  createdAt: string;
  userName: string | null;
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
      {value > 0 && <span className="ml-2 text-sm text-muted-foreground self-center">{value}/10</span>}
    </div>
  );
}

export function MangaDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { user, isAuthenticated } = useAuthStore();

  const [manga, setManga] = useState<MangaDetails | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSyncing, setIsSyncing] = useState(false);
  const [isFavorite, setIsFavorite] = useState(false);
  const [isFavoriteLoading, setIsFavoriteLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [reviews, setReviews] = useState<MangaReview[]>([]);
  const [reviewContent, setReviewContent] = useState("");
  const [reviewRating, setReviewRating] = useState(0);
  const [isSubmittingReview, setIsSubmittingReview] = useState(false);

  useEffect(() => {
    if (!id) return;
    let ignore = false;
    const fetchManga = async () => {
      try {
        setError(null);
        const response = await api.get<MangaDetails>(`/manga/${id}`);
        if (!ignore) {
          setManga(response.data);
          setIsLoading(false);
        }
      } catch (err) {
        if (!ignore) {
          if (axios.isAxiosError(err)) {
            setError(err.response?.data?.message || "Failed to fetch manga details.");
          } else {
            setError("An unexpected error occurred.");
          }
          setIsLoading(false);
        }
      }
    };
    fetchManga();
    return () => { ignore = true; };
  }, [id]);

  useEffect(() => {
    if (!id) return;
    const fetchReviews = async () => {
      try {
        const response = await api.get<MangaReview[]>(`/manga/${id}/reviews`);
        setReviews(response.data);
      } catch {
        // silently fail
      }
    };
    fetchReviews();
  }, [id]);

  useEffect(() => {
    if (!id || !user || !isAuthenticated) return;
    const checkFavorite = async () => {
      try {
        const response = await api.get<{ isFavorite: boolean }>(
          `/manga/favorites/check?userId=${user.id}&mangaId=${id}`
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
      await api.post(`/manga/${id}/sync`);
      const response = await api.get<MangaDetails>(`/manga/${id}`);
      setManga(response.data);
      toast.success("Manga synced from Jikan!");
    } catch {
      toast.error("Failed to sync manga.");
    } finally {
      setIsSyncing(false);
    }
  };

  const handleFavorite = async () => {
    if (!isAuthenticated || !user) {
      toast.error("Please sign in to add favorites.");
      navigate("/login");
      return;
    }
    if (!manga) return;
    setIsFavoriteLoading(true);
    try {
      if (isFavorite) {
        await api.delete(`/manga/favorites?userId=${user.id}&mangaId=${manga.mangaId}`);
        setIsFavorite(false);
        toast.success("Removed from favorites.");
      } else {
        await api.post("/manga/favorites", { userId: user.id, mangaId: manga.mangaId });
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
    if (!manga) return;
    if (!reviewContent.trim()) { toast.error("Please write something."); return; }
    if (reviewRating === 0) { toast.error("Please select a rating."); return; }

    setIsSubmittingReview(true);
    try {
      const response = await api.post<MangaReview>("/manga/reviews", {
        userId: user.id,
        mangaId: manga.mangaId,
        content: reviewContent,
        rating: reviewRating,
      });
      setReviews((prev) => [response.data, ...prev]);
      setReviewContent("");
      setReviewRating(0);
      toast.success("Review submitted!");
    } catch {
      toast.error("Failed to submit review.");
    } finally {
      setIsSubmittingReview(false);
    }
  };

  const handleDeleteReview = async (reviewId: number) => {
    try {
      await api.delete(`/manga/reviews/${reviewId}`);
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

  if (error || !manga) {
    return (
      <div className="flex flex-col items-center justify-center min-h-[50vh] text-center p-8">
        <h2 className="text-2xl font-bold text-destructive mb-2">Manga Not Found</h2>
        <p className="text-muted-foreground mb-6">{error || "The requested manga could not be found."}</p>
        <Button onClick={() => navigate("/manga")} variant="outline">
          <ArrowLeft className="mr-2 h-4 w-4" /> Back to Manga
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
        {/* Left Column */}
        <div className="md:col-span-1">
          <div className="relative aspect-[3/4] overflow-hidden rounded-lg border bg-muted shadow-lg">
            <img
              src={manga.imageURL || "https://via.placeholder.com/400x600?text=No+Image"}
              alt={manga.title}
              className="h-full w-full object-cover"
            />
          </div>
          <div className="mt-4 space-y-3">
            <Button className="w-full" onClick={handleSync} disabled={isSyncing}>
              {isSyncing ? <RefreshCw className="mr-2 h-4 w-4 animate-spin" /> : <RefreshCw className="mr-2 h-4 w-4" />}
              {isSyncing ? "Syncing..." : "Sync from Jikan"}
            </Button>
            <Button
              variant={isFavorite ? "default" : "outline"}
              className={`w-full ${isFavorite ? "text-white" : ""}`}
              onClick={handleFavorite}
              disabled={isFavoriteLoading}
            >
              <Heart className={`mr-2 h-4 w-4 ${isFavorite ? "fill-current" : ""}`} />
              {isFavoriteLoading ? "Updating..." : isFavorite ? "Remove from Favorites" : "Add to Favorites"}
            </Button>
          </div>
        </div>

        {/* Right Column */}
        <div className="md:col-span-2 space-y-6">
          <div>
            <h1 className="text-3xl md:text-4xl font-bold tracking-tight mb-2">{manga.title}</h1>
            {manga.score > 0 && (
              <Badge variant="secondary" className="text-base px-3 py-1 mb-4">
                <Star className="mr-1.5 h-4 w-4 fill-yellow-400 text-yellow-400" />
                {manga.score.toFixed(1)} / 10
              </Badge>
            )}
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div className="flex items-center gap-2 text-muted-foreground">
                <BookOpen className="h-4 w-4 text-primary" />
                <span><strong className="text-foreground">Type:</strong> {manga.type || "Unknown"}</span>
              </div>
              <div className="flex items-center gap-2 text-muted-foreground">
                <BookOpen className="h-4 w-4 text-primary" />
                <span><strong className="text-foreground">Chapters:</strong> {manga.chapters > 0 ? manga.chapters : "?"}</span>
              </div>
              <div className="flex items-center gap-2 text-muted-foreground">
                <BookOpen className="h-4 w-4 text-primary" />
                <span><strong className="text-foreground">Volumes:</strong> {manga.volumes > 0 ? manga.volumes : "?"}</span>
              </div>
              <div className="flex items-center gap-2 text-muted-foreground">
                <BookOpen className="h-4 w-4 text-primary" />
                <span><strong className="text-foreground">Status:</strong> {manga.status || "Unknown"}</span>
              </div>
              <div className="flex items-center gap-2 text-muted-foreground col-span-2">
                <BookOpen className="h-4 w-4 text-primary" />
                <span>
                  <strong className="text-foreground">Published:</strong>{" "}
                  {manga.publishedFrom ? new Date(manga.publishedFrom).getFullYear() : "?"} -{" "}
                  {manga.publishedTo ? new Date(manga.publishedTo).getFullYear() : "?"}
                </span>
              </div>
            </div>
          </div>

          <Separator />

          <div>
            <h2 className="text-xl font-semibold mb-3">Synopsis</h2>
            <p className="text-muted-foreground leading-relaxed whitespace-pre-line">
              {manga.synopsis || "No synopsis available."}
            </p>
          </div>
        </div>
      </div>

      {/* Reviews */}
      <Separator />
      <div className="space-y-6">
        <h2 className="text-2xl font-bold">Reviews</h2>

        {isAuthenticated ? (
          <div className="border rounded-lg p-6 space-y-4 bg-card">
            <h3 className="text-lg font-semibold">Write a Review</h3>
            <StarRating value={reviewRating} onChange={setReviewRating} />
            <Textarea
              placeholder="Share your thoughts about this manga..."
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
              <button onClick={() => navigate("/login")} className="text-primary underline">Sign in</button>{" "}
              to leave a review.
            </p>
          </div>
        )}

        {reviews.length === 0 ? (
          <p className="text-muted-foreground text-center py-8">No reviews yet. Be the first!</p>
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
                      <p className="text-xs text-muted-foreground">{new Date(review.createdAt).toLocaleDateString()}</p>
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