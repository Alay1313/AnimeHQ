using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class MangaFavoriteRepo : IMangaFavoriteRepo
{
    private readonly AppDbContext _context;

    public MangaFavoriteRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MangaFavorite?> GetByUserAndMangaIdAsync(string userId, int mangaId, CancellationToken ct = default)
    {
        return await _context.MangaFavorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.MangaId == mangaId, ct);
    }

    public async Task<IEnumerable<MangaFavorite>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _context.MangaFavorites
            .Include(f => f.Manga)
            .Where(f => f.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<MangaFavorite> CreateAsync(MangaFavorite favorite, CancellationToken ct = default)
    {
        await _context.MangaFavorites.AddAsync(favorite, ct);
        await _context.SaveChangesAsync(ct);
        return favorite;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var favorite = await _context.MangaFavorites.FindAsync(new object[] { id }, ct);
        if (favorite == null) return false;
        _context.MangaFavorites.Remove(favorite);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}