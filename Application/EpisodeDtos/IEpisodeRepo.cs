using Domain;

namespace Application;

public interface IEpisodeRepo
{
    Task<Episode?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Episode>> GetByAnimeIdAsync(int animeId, CancellationToken ct = default);
    Task<IEnumerable<Episode>> GetByAnimeIdPagedAsync(int animeId, int page, int pageSize, CancellationToken ct = default);
    Task<Episode> CreateAsync(Episode episode, CancellationToken ct = default);
    Task<Episode?> UpdateAsync(int id, Episode updatedEpisode, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> DeleteByAnimeIdAsync(int animeId, CancellationToken ct = default);
}
