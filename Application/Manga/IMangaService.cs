namespace Application;

public interface IMangaService
{
    Task<MangaDto> GetByIdAsync(int mangaId, CancellationToken ct = default);
    Task<IEnumerable<MangaDto>> SearchAsync(string query, int page = 1, int pageSize = 25, CancellationToken ct = default);
    Task<IEnumerable<MangaDto>> GetTopMangaAsync(int page = 1, int pageSize = 25, CancellationToken ct = default);
    Task<MangaDto> SyncFromJikanAsync(int mangaId, CancellationToken ct = default);
    Task<MangaFavoriteDto?> AddFavoriteAsync(CreateMangaFavoriteDto dto, CancellationToken ct = default);
    Task<bool> RemoveFavoriteAsync(string userId, int mangaId, CancellationToken ct = default);
    Task<bool> IsFavoriteAsync(string userId, int mangaId, CancellationToken ct = default);
    Task<MangaReviewDto?> CreateReviewAsync(CreateMangaReviewDto dto, CancellationToken ct = default);
    Task<IEnumerable<MangaReviewDto>> GetReviewsAsync(int mangaId, CancellationToken ct = default);
    Task<bool> DeleteReviewAsync(int reviewId, CancellationToken ct = default);
    Task<IEnumerable<MangaFavoriteDto>> GetUserFavoritesAsync(string userId, CancellationToken ct = default);
    Task<IEnumerable<MangaReviewDto>> GetUserReviewsAsync(string userId, CancellationToken ct = default);
}