import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Star, Tv, Film } from "lucide-react";
import { Link } from "react-router-dom";

export interface Anime {
  animeListId: number;
  title: string;
  imageURL: string;
  score: number;
  type: string;
  episodes: number;
  status: string;
}

interface AnimeCardProps {
  anime: Anime;
}

export function AnimeCard({ anime }: AnimeCardProps) {
  const fallbackImage = "https://via.placeholder.com/400x600/18181b/ffffff?text=No+Image";
  const imageUrl = anime.imageURL || fallbackImage;

  return (
    <Card className="group overflow-hidden transition-all duration-300 hover:shadow-2xl hover:-translate-y-2 bg-card border-muted h-full flex flex-col">
      
      {/* Image Container*/}
      <div className="relative aspect-[3/4] overflow-hidden bg-muted">
        <img 
          src={imageUrl} 
          alt={anime.title}
          className="h-full w-full object-cover transition-transform duration-700 group-hover:scale-110"
          loading="lazy"
          onError={(e) => {
            (e.target as HTMLImageElement).src = fallbackImage;
          }}
        />
        
        {/* Score Badge */}
        {anime.score > 0 && (
          <div className="absolute top-3 right-3 flex items-center gap-1.5 bg-black/80 text-white px-2.5 py-1.5 rounded-lg text-sm font-bold backdrop-blur-md shadow-lg border border-white/10">
            <Star className="h-4 w-4 fill-yellow-400 text-yellow-400" />
            {anime.score.toFixed(1)}
          </div>
        )}
      </div>

      {/* Title Section*/}
      <CardHeader className="p-5 pb-3 flex-grow">
        <CardTitle 
          className="text-lg font-bold line-clamp-2 leading-snug text-foreground group-hover:text-primary transition-colors" 
          title={anime.title}
        >
          {anime.title}
        </CardTitle>
      </CardHeader>

      {/* Metadata Section */}
      <CardContent className="p-5 pt-0 space-y-4">
        <div className="flex items-center gap-5 text-sm font-medium text-muted-foreground">
          <div className="flex items-center gap-2">
            <Tv className="h-4 w-4 text-primary/80" />
            <span>{anime.type || "Unknown"}</span>
          </div>
          <div className="flex items-center gap-2">
            <Film className="h-4 w-4 text-primary/80" />
            <span>{anime.episodes > 0 ? `${anime.episodes} Episodes` : "?"}</span>
          </div>
        </div>
      </CardContent>

      {/* Action Button */}
      <CardFooter className="p-5 pt-0 mt-auto">
        <Link to={`/anime/${anime.animeListId}`} className="w-full">
          <Button variant="secondary" className="w-full h-11 text-base font-semibold group-hover:bg-primary group-hover:text-primary-foreground transition-all duration-300">
            View Details
          </Button>
        </Link>
      </CardFooter>
    </Card>
  );
}