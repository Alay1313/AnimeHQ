import { AnimeListPage } from "./AnimeListPage";
export function Popular() {
  return <AnimeListPage title="Popular" description="The most popular anime of all time." endpoint="/anime/popular?page=1&pageSize=20" />;
}