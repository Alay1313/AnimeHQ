using System;
using Domain;
namespace Application;

public interface IEpisodeRepo
{
     Task<Episode?> GetByIdAsync(int id);
    Task<IEnumerable<Episode>> GetByAnimeIdAsync(int animeId);
    Task<IEnumerable<Episode>> GetByAnimeIdAsync(int animeId, int page, int pageSize);
    Task<Episode> CreateAsync(Episode episode);
    Task<Episode?> UpdateAsync(int id, Episode updatedEpisode);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByAnimeIdAsync(int animeId);

}
