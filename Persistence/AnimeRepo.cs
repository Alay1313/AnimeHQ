using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AnimeRepo : IAnimeRepo
{
    // Note: Change 'AppDbContext' to whatever your actual DbContext class is named
    private readonly AppDbContext _context;

    public AnimeRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Anime?> GetByIdAsync(int animeListId, CancellationToken ct = default)
    {
        return await _context.Animes
            .Include(a => a.AnimeGenres) // Required for AutoMapper to map GenreIds
            .FirstOrDefaultAsync(a => a.AnimeListId == animeListId, ct);
    }

    public async Task<IEnumerable<Anime>> SearchAsync(string query, int page, int pageSize, CancellationToken ct = default)
    {
        var skip = (page - 1) * pageSize;
        var lowerQuery = query.ToLower();

        return await _context.Animes
            .Include(a => a.AnimeGenres) // Required for AutoMapper to map GenreIds
            .Where(a => a.Title.ToLower().Contains(lowerQuery))
            .OrderBy(a => a.Title)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<Anime> CreateAsync(Anime anime, CancellationToken ct = default)
    {
        await _context.Animes.AddAsync(anime, ct);
        await _context.SaveChangesAsync(ct);
        
        // Reload to ensure we have the generated IDs and tracked collections
        return await GetByIdAsync(anime.AnimeListId, ct) ?? anime;
    }

    public async Task<Anime> UpdateAsync(int animeListId, Anime anime, CancellationToken ct = default)
    {
        // EF Core will track this entity and update its scalar properties.
        // Because we clear and rebuild the AnimeGenres collection in the Service layer, 
        // EF Core will automatically handle the INSERTs/DELETEs for the join table.
        _context.Animes.Update(anime);
        await _context.SaveChangesAsync(ct);
        
        return await GetByIdAsync(animeListId, ct) ?? anime;
    }

    public async Task<bool> DeleteAsync(int animeListId, CancellationToken ct = default)
    {
        var anime = await _context.Animes.FindAsync(new object[] { animeListId }, ct);
        if (anime == null)
        {
            return false;
        }

        _context.Animes.Remove(anime);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
