import { AnimeListPage } from "./AnimeListPage";
export function TopAiring() {
  return <AnimeListPage title="Top Airing" description="The most popular anime currently broadcasting." endpoint="/anime/top-airing?page=1&pageSize=20" />;
}