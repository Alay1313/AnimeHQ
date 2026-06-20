import { useEffect, useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import api from "@/lib/api";
import { useAuthStore } from "@/store/authStore";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Star, Heart, BookOpen, Tv, Trash2 } from "lucide-react";
import { toast } from "sonner";

interface AnimeFavorite {
  id: number;
  animeId: number;
  userId: string;
  createdAt: string;
  animeTitle: string | null;
}

interface MangaFavorite {
  id: number;
  mangaId: number;
  userId: string;
  createdAt: string;
  mangaTitle: string | null;
}

interface AnimeReview {
  reviewId: number;
  animeId: number;
  userId: string;
  content: string;
  rating: number;
  createdAt: string;
  animeTitle: string | null;
  userName: string | null;
}

interface MangaReview {
  reviewId: number;
  mangaId: number;
  userId: string;
  content: string;
  rating: number;
  createdAt: string;
  mangaTitle: string | null;
  userName: string | null;
}

type Tab = "anime-favorites" | "manga-favorites" | "anime-reviews" | "manga-reviews";

export function MyList() {
  const { user, isAuthenticated } = useAuthStore();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<Tab>("anime-favorites");

  const [animeFavorites, setAnimeFavorites] = useState<AnimeFavorite[]>([]);
  const [mangaFavorites, setMangaFavorites] = useState<MangaFavorite[]>([]);
  const [animeReviews, setAnimeReviews] = useState<AnimeReview[]>([]);
  const [mangaReviews, setMangaReviews] = useState<MangaReview[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    if (!isAuthenticated || !user) {
      navigate("/login");
      return;
    }

    const fetchAll = async () => {
      setIsLoading(true);
      try {
        const [afRes, mfRes, arRes, mrRes] = await Promise.all([
          api.get<AnimeFavorite[]>(`/favorite/user/${user.id}`),
          api.get<MangaFavorite[]>(`/manga/favorites/user/${user.id}`),
          api.get<AnimeReview[]>(`/review/user/${user.id}`),
          api.get<MangaReview[]>(`/manga/reviews/user/${user.id}`),
        ]);
        setAnimeFavorites(afRes.data);
        setMangaFavorites(mfRes.data);
        setAnimeReviews(arRes.data);
        setMangaReviews(mrRes.data);
      } catch {
        toast.error("Failed to load your list.");
      } finally {
        setIsLoading(false);
      }
    };

    fetchAll();
  }, [user, isAuthenticated, navigate]);

  const handleRemoveAnimeFavorite = async (animeId: number) => {
    if (!user) return;
    try {
      await api.delete(`/favorite?userId=${user.id}&animeId=${animeId}`);
      setAnimeFavorites((prev) => prev.filter((f) => f.animeId !== animeId));
      toast.success("Removed from favorites.");
    } catch {
      toast.error("Failed to remove favorite.");
    }
  };

  const handleRemoveMangaFavorite = async (mangaId: number) => {
    if (!user) return;
    try {
      await api.delete(`/manga/favorites?userId=${user.id}&mangaId=${mangaId}`);
      setMangaFavorites((prev) => prev.filter((f) => f.mangaId !== mangaId));
      toast.success("Removed from favorites.");
    } catch {
      toast.error("Failed to remove favorite.");
    }
  };

  const handleDeleteAnimeReview = async (reviewId: number) => {
    try {
      await api.delete(`/review/${reviewId}`);
      setAnimeReviews((prev) => prev.filter((r) => r.reviewId !== reviewId));
      toast.success("Review deleted.");
    } catch {
      toast.error("Failed to delete review.");
    }
  };

  const handleDeleteMangaReview = async (reviewId: number) => {
    try {
      await api.delete(`/manga/reviews/${reviewId}`);
      setMangaReviews((prev) => prev.filter((r) => r.reviewId !== reviewId));
      toast.success("Review deleted.");
    } catch {
      toast.error("Failed to delete review.");
    }
  };

  const tabs: { id: Tab; label: string; count: number }[] = [
    { id: "anime-favorites", label: "Anime Favorites", count: animeFavorites.length },
    { id: "manga-favorites", label: "Manga Favorites", count: mangaFavorites.length },
    { id: "anime-reviews", label: "Anime Reviews", count: animeReviews.length },
    { id: "manga-reviews", label: "Manga Reviews", count: mangaReviews.length },
  ];

  return (
    <div className="container mx-auto px-4 py-8 max-w-5xl space-y-8">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">My List</h1>
        <p className="text-muted-foreground">Your favorites and reviews all in one place.</p>
      </div>

      {/* Tabs */}
      <div className="flex flex-wrap gap-2 border-b">
        {tabs.map((tab) => (
          <button
            key={tab.id}
            onClick={() => setActiveTab(tab.id)}
            className={`px-4 py-2 text-sm font-medium transition-colors border-b-2 -mb-px ${
              activeTab === tab.id
                ? "border-primary text-primary"
                : "border-transparent text-muted-foreground hover:text-foreground"
            }`}
          >
            {tab.label}
            {!isLoading && (
              <span className="ml-2 text-xs bg-muted px-1.5 py-0.5 rounded-full">
                {tab.count}
              </span>
            )}
          </button>
        ))}
      </div>

      {/* Content */}
      {isLoading ? (
        <div className="space-y-4">
          {Array.from({ length: 4 }).map((_, i) => (
            <Skeleton key={i} className="h-24 w-full rounded-lg" />
          ))}
        </div>
      ) : (
        <div className="space-y-4">

          {/* Anime Favorites */}
          {activeTab === "anime-favorites" && (
            animeFavorites.length === 0 ? (
              <div className="text-center py-16 text-muted-foreground">
                <Heart className="h-12 w-12 mx-auto mb-4 opacity-20" />
                <p className="text-lg">No anime favorites yet.</p>
                <Link to="/" className="text-primary underline text-sm mt-2 inline-block">
                  Browse anime
                </Link>
              </div>
            ) : (
              animeFavorites.map((fav) => (
                <div key={fav.id} className="flex items-center justify-between p-4 border rounded-lg bg-card hover:bg-muted/50 transition-colors">
                  <div className="flex items-center gap-3">
                    <Tv className="h-5 w-5 text-primary flex-shrink-0" />
                    <div>
                      <Link
                        to={`/anime/${fav.animeId}`}
                        className="font-semibold hover:text-primary transition-colors"
                      >
                        {fav.animeTitle ?? `Anime #${fav.animeId}`}
                      </Link>
                      <p className="text-xs text-muted-foreground">
                        Added {new Date(fav.createdAt).toLocaleDateString()}
                      </p>
                    </div>
                  </div>
                  <Button
                    variant="ghost"
                    size="sm"
                    className="text-destructive hover:text-destructive"
                    onClick={() => handleRemoveAnimeFavorite(fav.animeId)}
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </div>
              ))
            )
          )}

          {/* Manga Favorites */}
          {activeTab === "manga-favorites" && (
            mangaFavorites.length === 0 ? (
              <div className="text-center py-16 text-muted-foreground">
                <Heart className="h-12 w-12 mx-auto mb-4 opacity-20" />
                <p className="text-lg">No manga favorites yet.</p>
                <Link to="/manga" className="text-primary underline text-sm mt-2 inline-block">
                  Browse manga
                </Link>
              </div>
            ) : (
              mangaFavorites.map((fav) => (
                <div key={fav.id} className="flex items-center justify-between p-4 border rounded-lg bg-card hover:bg-muted/50 transition-colors">
                  <div className="flex items-center gap-3">
                    <BookOpen className="h-5 w-5 text-primary flex-shrink-0" />
                    <div>
                      <Link
                        to={`/manga/${fav.mangaId}`}
                        className="font-semibold hover:text-primary transition-colors"
                      >
                        {fav.mangaTitle ?? `Manga #${fav.mangaId}`}
                      </Link>
                      <p className="text-xs text-muted-foreground">
                        Added {new Date(fav.createdAt).toLocaleDateString()}
                      </p>
                    </div>
                  </div>
                  <Button
                    variant="ghost"
                    size="sm"
                    className="text-destructive hover:text-destructive"
                    onClick={() => handleRemoveMangaFavorite(fav.mangaId)}
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </div>
              ))
            )
          )}

          {/* Anime Reviews */}
          {activeTab === "anime-reviews" && (
            animeReviews.length === 0 ? (
              <div className="text-center py-16 text-muted-foreground">
                <Star className="h-12 w-12 mx-auto mb-4 opacity-20" />
                <p className="text-lg">No anime reviews yet.</p>
                <Link to="/" className="text-primary underline text-sm mt-2 inline-block">
                  Browse anime to review
                </Link>
              </div>
            ) : (
              animeReviews.map((review) => (
                <div key={review.reviewId} className="p-5 border rounded-lg bg-card space-y-3">
                  <div className="flex items-center justify-between">
                    <Link
                      to={`/anime/${review.animeId}`}
                      className="font-semibold hover:text-primary transition-colors"
                    >
                      {review.animeTitle ?? `Anime #${review.animeId}`}
                    </Link>
                    <div className="flex items-center gap-3">
                      <Badge variant="secondary" className="flex items-center gap-1">
                        <Star className="h-3 w-3 fill-yellow-400 text-yellow-400" />
                        {review.rating}/10
                      </Badge>
                      <Button
                        variant="ghost"
                        size="sm"
                        className="text-destructive hover:text-destructive"
                        onClick={() => handleDeleteAnimeReview(review.reviewId)}
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>
                  <p className="text-muted-foreground text-sm leading-relaxed">{review.content}</p>
                  <p className="text-xs text-muted-foreground">
                    {new Date(review.createdAt).toLocaleDateString()}
                  </p>
                </div>
              ))
            )
          )}

          {/* Manga Reviews */}
          {activeTab === "manga-reviews" && (
            mangaReviews.length === 0 ? (
              <div className="text-center py-16 text-muted-foreground">
                <Star className="h-12 w-12 mx-auto mb-4 opacity-20" />
                <p className="text-lg">No manga reviews yet.</p>
                <Link to="/manga" className="text-primary underline text-sm mt-2 inline-block">
                  Browse manga to review
                </Link>
              </div>
            ) : (
              mangaReviews.map((review) => (
                <div key={review.reviewId} className="p-5 border rounded-lg bg-card space-y-3">
                  <div className="flex items-center justify-between">
                    <Link
                      to={`/manga/${review.mangaId}`}
                      className="font-semibold hover:text-primary transition-colors"
                    >
                      {review.mangaTitle ?? `Manga #${review.mangaId}`}
                    </Link>
                    <div className="flex items-center gap-3">
                      <Badge variant="secondary" className="flex items-center gap-1">
                        <Star className="h-3 w-3 fill-yellow-400 text-yellow-400" />
                        {review.rating}/10
                      </Badge>
                      <Button
                        variant="ghost"
                        size="sm"
                        className="text-destructive hover:text-destructive"
                        onClick={() => handleDeleteMangaReview(review.reviewId)}
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </div>
                  <p className="text-muted-foreground text-sm leading-relaxed">{review.content}</p>
                  <p className="text-xs text-muted-foreground">
                    {new Date(review.createdAt).toLocaleDateString()}
                  </p>
                </div>
              ))
            )
          )}
        </div>
      )}
    </div>
  );
}