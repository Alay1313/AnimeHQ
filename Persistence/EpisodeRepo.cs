using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application;

public class EpisodeRepo : IEpisodeRepo
{
    private readonly AppDbContext _context;

    public EpisodeRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Episode?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Episodes
            .Include(e => e.Anime)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IEnumerable<Episode>> GetByAnimeIdAsync(int animeId, CancellationToken ct = default)
    {
        return await _context.Episodes
            .Where(e => e.AnimeId == animeId)
            .OrderBy(e => e.EpisodeNumber)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Episode>> GetByAnimeIdPagedAsync(int animeId, int page, int pageSize, CancellationToken ct = default)
    {
        return await _context.Episodes
            .Where(e => e.AnimeId == animeId)
            .OrderBy(e => e.EpisodeNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<Episode> CreateAsync(Episode episode, CancellationToken ct = default)
    {
        var entity = await _context.Episodes.AddAsync(episode, ct);
        await _context.SaveChangesAsync(ct);
        return entity.Entity;
    }

    public async Task<Episode?> UpdateAsync(int id, Episode updatedEpisode, CancellationToken ct = default)
    {
        var existing = await GetByIdAsync(id, ct);
        if (existing == null) return null;

        existing.Title = updatedEpisode.Title;
        existing.EpisodeNumber = updatedEpisode.EpisodeNumber;
        existing.AirDate = updatedEpisode.AirDate;
        existing.Synopsis = updatedEpisode.Synopsis;
        existing.Duration = updatedEpisode.Duration;

        await _context.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var episode = await GetByIdAsync(id, ct);
        if (episode == null) return false;

        _context.Episodes.Remove(episode);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteByAnimeIdAsync(int animeId, CancellationToken ct = default)
    {
        var episodes = await _context.Episodes.Where(e => e.AnimeId == animeId).ToListAsync(ct);
        if (episodes.Count == 0) return false;

        _context.Episodes.RemoveRange(episodes);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}