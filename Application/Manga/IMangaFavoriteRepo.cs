using Domain;

namespace Application;

public interface IMangaFavoriteRepo
{
    Task<MangaFavorite?> GetByUserAndMangaIdAsync(string userId, int mangaId, CancellationToken ct = default);
    Task<IEnumerable<MangaFavorite>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<MangaFavorite> CreateAsync(MangaFavorite favorite, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}