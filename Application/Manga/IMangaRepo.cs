using Domain;

namespace Application;

public interface IMangaRepo
{
    Task<Manga?> GetByIdAsync(int mangaId, CancellationToken ct = default);
    Task<Manga> CreateAsync(Manga manga, CancellationToken ct = default);
    Task<Manga> UpdateAsync(int mangaId, Manga manga, CancellationToken ct = default);
    Task<IEnumerable<Manga>> SearchAsync(string query, int page, int pageSize, CancellationToken ct = default);
    Task<IEnumerable<Manga>> GetTopCachedAsync(int page, int pageSize, CancellationToken ct = default);
}