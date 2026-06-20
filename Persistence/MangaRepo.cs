using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class MangaRepo : IMangaRepo
{
    private readonly AppDbContext _context;

    public MangaRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Manga?> GetByIdAsync(int mangaId, CancellationToken ct = default)
    {
        return await _context.Mangas.FirstOrDefaultAsync(m => m.MangaId == mangaId, ct);
    }

    public async Task<Manga> CreateAsync(Manga manga, CancellationToken ct = default)
    {
        await _context.Mangas.AddAsync(manga, ct);
        await _context.SaveChangesAsync(ct);
        return manga;
    }

   public async Task<Manga> UpdateAsync(int mangaId, Manga manga, CancellationToken ct = default)
{
    var existing = await _context.Mangas.FirstOrDefaultAsync(m => m.MangaId == mangaId, ct);
    if (existing == null) return manga;

    // Update properties on the tracked entity
    existing.Title = manga.Title;
    existing.ImageURL = manga.ImageURL;
    existing.Score = manga.Score;
    existing.Synopsis = manga.Synopsis;
    existing.Type = manga.Type;
    existing.Chapters = manga.Chapters;
    existing.Volumes = manga.Volumes;
    existing.Status = manga.Status;
    existing.PublishedFrom = manga.PublishedFrom;
    existing.PublishedTo = manga.PublishedTo;
    existing.CachedAt = manga.CachedAt;

    await _context.SaveChangesAsync(ct);
    return existing;
}


    public async Task<IEnumerable<Manga>> SearchAsync(string query, int page, int pageSize, CancellationToken ct = default)
    {
        var queryable = _context.Mangas.AsQueryable();

        // If the user is searching for a specific title, filter the local cache
        if (!string.IsNullOrWhiteSpace(query))
        {
            queryable = queryable.Where(m => m.Title.ToLower().Contains(query.ToLower()));
        }

        return await queryable
            .OrderByDescending(m => m.Score) // Sort by highest rated
            .Skip((page - 1) * pageSize)     // Pagination
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Manga>> GetTopCachedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        // Simply grab the highest-rated manga from our local database
        return await _context.Mangas
            .OrderByDescending(m => m.Score)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }
}