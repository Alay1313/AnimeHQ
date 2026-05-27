using System;

using Domain;

namespace Persistence;

public interface IReviewRepo
{
    Task<Review?> GetByIdAsync(int id);
    Task<IEnumerable<Review>> GetByAnimeIdAsync(int animeId);
    Task<IEnumerable<Review>> GetByUserIdAsync(string userId);
    Task<Review> CreateAsync(Review review);
    Task<Review?> UpdateAsync(int id, Review updatedReview);
    Task<bool> DeleteAsync(int id);

}
