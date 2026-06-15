

namespace Application;

public interface IReviewService
{
    Task<ReviewDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<ReviewDto>> GetByAnimeIdAsync(int animeId, CancellationToken ct = default);
    Task<IEnumerable<ReviewDto>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<ReviewDto?> CreateAsync(CreateReviewDto dto, CancellationToken ct = default);
    Task<ReviewDto?> UpdateAsync(int id, UpdateReviewDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
