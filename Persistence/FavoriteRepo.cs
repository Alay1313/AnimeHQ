using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application;

public class FavoriteRepo : IFavoriteRepo
{
    private readonly AppDbContext _context;

    public FavoriteRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Favorite?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Favorites
            .Include(f => f.User)
            .Include(f => f.Anime)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task<IEnumerable<Favorite>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _context.Favorites
            .Include(f => f.Anime)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Favorite?> GetByUserAndAnimeIdAsync(string userId, int animeId, CancellationToken ct = default)
    {
        return await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.AnimeId == animeId, ct);
    }

    public async Task<Favorite> CreateAsync(Favorite favorite, CancellationToken ct = default)
    {
        var e = await _context.Favorites.AddAsync(favorite, ct);
        await _context.SaveChangesAsync(ct); 
        return e.Entity; 
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var favorite = await GetByIdAsync(id, ct); 
        if (favorite == null) return false;
        
        _context.Favorites.Remove(favorite); 
        await _context.SaveChangesAsync(ct);
        return true; 
    }
}