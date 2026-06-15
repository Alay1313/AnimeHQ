import { AnimeListPage } from "./AnimeListPage";
export function Seasonal() {
  return <AnimeListPage title="Seasonal" description="Anime airing this season." endpoint="/anime/seasonal?page=1&pageSize=20" />;
}