
using Domain;

namespace Persistence;

public interface IReviewRepo
{
    Task<Review?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Review>> GetByAnimeIdAsync(int animeId, CancellationToken ct = default);
    Task<IEnumerable<Review>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<Review> CreateAsync(Review review, CancellationToken ct = default);
    Task<Review?> UpdateAsync(int id, Review updatedReview, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}