using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class MangaReviewRepo : IMangaReviewRepo
{
    private readonly AppDbContext _context;

    public MangaReviewRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MangaReview?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.MangaReviews
            .Include(r => r.User)
            .Include(r => r.Manga)
            .FirstOrDefaultAsync(r => r.ReviewId == id, ct);
    }

    public async Task<IEnumerable<MangaReview>> GetByMangaIdAsync(int mangaId, CancellationToken ct = default)
    {
        return await _context.MangaReviews
            .Include(r => r.User)
            .Where(r => r.MangaId == mangaId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<MangaReview> CreateAsync(MangaReview review, CancellationToken ct = default)
    {
        await _context.MangaReviews.AddAsync(review, ct);
        await _context.SaveChangesAsync(ct);
        return await GetByIdAsync(review.ReviewId, ct) ?? review;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var review = await _context.MangaReviews.FindAsync(new object[] { id }, ct);
        if (review == null) return false;
        _context.MangaReviews.Remove(review);
        await _context.SaveChangesAsync(ct);
        return true;
    }


    public async Task<IEnumerable<MangaReview>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _context.MangaReviews
            .Include(r => r.Manga)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }
}