using System;
using Domain;
namespace Application;

public interface IAnimeRepo
{
    Task<Anime?> GetByIdAsync(int AnimeListId);
    Task<IEnumerable<Anime>> SearchAsync(string query, int page = 1, int pageSize = 20);
    Task<Anime> CreateAsync(Anime anime);
    Task<Anime?> UpdateAsync(int AnimeListId, Anime updatedAnime);
    Task<bool> DeleteAsync(int AnimeListId);

    Task<IEnumerable<JikanAnimeData>> GetTopAiringFromJikanAsync(int page = 1, int pageSize = 25, CancellationToken ct = default);
    


}
 