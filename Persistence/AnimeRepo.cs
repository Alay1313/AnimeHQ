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

    public async Task<IEnumerable<Anime>> SearchAsync(string query, int page, int pageSize, List<int>? genreIds = null, CancellationToken ct = default)
    {
        var skip = (page - 1) * pageSize;

        var queryable = _context.Animes
            .Include(a => a.AnimeGenres)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            queryable = queryable.Where(a => a.Title.ToLower().Contains(lowerQuery));
        }

        // Filter by genre — anime must match AT LEAST ONE selected genre
        if (genreIds != null && genreIds.Count > 0)
        {
            queryable = queryable.Where(a =>
            a.AnimeGenres.Any(ag => genreIds.Contains(ag.GenreId)));
        }

        return await queryable
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
        var existing = await _context.Animes
            .Include(a => a.AnimeGenres)
            .FirstOrDefaultAsync(a => a.AnimeListId == animeListId, ct);
    
        if (existing == null) return anime;

        existing.Title = anime.Title;
        existing.ImageURL = anime.ImageURL;
        existing.Score = anime.Score;
        existing.Synopsis = anime.Synopsis;
        existing.Type = anime.Type;
        existing.Episodes = anime.Episodes;
        existing.Status = anime.Status;
        existing.AiredFrom = anime.AiredFrom;
        existing.AiredTo = anime.AiredTo;
        existing.CachedAt = anime.CachedAt;

        await _context.SaveChangesAsync(ct);
        return await GetByIdAsync(animeListId, ct) ?? existing;
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
