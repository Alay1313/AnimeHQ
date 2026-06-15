

namespace Application;

public interface IEpisodeService
{
    Task<EpisodeDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<EpisodeDto>> GetByAnimeIdAsync(int animeId, int page, int pageSize, CancellationToken ct = default);
    Task<EpisodeDto?> CreateAsync(CreateEpisodeDto dto, CancellationToken ct = default);
    Task<EpisodeDto?> UpdateAsync(int id, UpdateEpisodeDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}