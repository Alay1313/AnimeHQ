using Domain;

namespace Application;

public interface IMangaReviewRepo
{
    Task<MangaReview?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<MangaReview>> GetByMangaIdAsync(int mangaId, CancellationToken ct = default);
    Task<MangaReview> CreateAsync(MangaReview review, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<MangaReview>> GetByUserIdAsync(string userId, CancellationToken ct = default);
}