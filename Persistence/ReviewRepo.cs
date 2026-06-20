using Domain;
using Microsoft.EntityFrameworkCore;
using Application;

namespace Persistence;

public class ReviewRepo : IReviewRepo
{
    private readonly AppDbContext _context;

    public ReviewRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Anime)
            .FirstOrDefaultAsync(r => r.ReviewId == id, ct);
    }

    public async Task<IEnumerable<Review>> GetByAnimeIdAsync(int animeId, CancellationToken ct = default)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.AnimeId == animeId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Review>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Anime)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Review> CreateAsync(Review review, CancellationToken ct = default)
    {
        var e = await _context.Reviews.AddAsync(review, ct);
        await _context.SaveChangesAsync(ct); 
        return e.Entity; 
    }

    public async Task<Review?> UpdateAsync(int id, Review updated, CancellationToken ct = default)
    {
        var existing = await GetByIdAsync(id, ct);
        if (existing == null) return null;

        existing.Content = updated.Content;
        existing.Rating = updated.Rating;

        await _context.SaveChangesAsync(ct); 
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var review = await GetByIdAsync(id, ct); 
        if (review == null) return false;
        
        _context.Reviews.Remove(review); 
        await _context.SaveChangesAsync(ct);
        return true; 
    }
}