
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application;

public class ReviewRepo(AppDbContext context) : IReviewRepo
{
    private readonly AppDbContext _context = context;

    public async Task<Review?> GetByIdAsync(int Id)
    {
        return  await _context.Reviews.Include(r => r.User).Include(r => r.Anime).FirstOrDefaultAsync(r => r.ReviewId == Id);
       
    }

    public async Task<IEnumerable<Review>> GetByAnimeIdAsync(int animeId)
    {
        return await _context.Reviews.Where(r => r.AnimeId == animeId).ToListAsync();
            
    }



      public async Task<IEnumerable<Review>> GetByUserIdAsync(string userId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Anime)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    

    public async Task<Review> CreateAsync(Review review)
    {
       
    var e = await _context.Reviews.AddAsync(review);
    await _context.SaveChangesAsync(); 
    return e.Entity; 
        
    }

    public async Task<Review?> UpdateAsync(int Id, Review updated)
    {
         var existing = await GetByIdAsync(Id);
         if (existing == null) return null;

         existing.Content = updated.Content;
         existing.Rating = updated.Rating;

         return existing;
    }

    public async Task<bool> DeleteAsync(int reviewId)
    {
        var r = await GetByIdAsync(reviewId); if (r == null) return false;
        _context.Reviews.Remove(r); await _context.SaveChangesAsync();
        return true; 
    }
}