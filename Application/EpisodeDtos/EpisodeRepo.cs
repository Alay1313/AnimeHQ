using System;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application;

public class EpisodeRepo(AppDbContext context) : IEpisodeRepo
{
    private readonly AppDbContext _context = context;



    public async Task<Episode?> GetByIdAsync(int id)
    {
        return await _context.Episodes
            .Include(e => e.Anime)
            .FirstOrDefaultAsync(e => e.Id == id);
        
    }


    public async Task<IEnumerable<Episode>> GetByAnimeIdAsync(int animeId)
    {
        
        return await _context.Episodes
            .Include(e => e.Anime)
            .Where(e => e.AnimeId == animeId)
            .OrderBy(e => e.EpisodeNumber)
            .ToListAsync();

    }

    public async Task<IEnumerable<Episode>> GetByAnimeIdAsync(int animeId, int page, int pageSize)
    {
        return await _context.Episodes
            .Include(e => e.Anime)
            .Where(e => e.AnimeId == animeId)
            .OrderBy(e => e.EpisodeNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Episode> CreateAsync(Episode episode)
    {
        var entity = await _context.Episodes.AddAsync(episode);
        await _context.SaveChangesAsync();
        return entity.Entity;
    }


    public async Task<Episode?> UpdateAsync(int id, Episode updatedEpisode)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) return null;

        existing.Title = updatedEpisode.Title;
        existing.EpisodeNumber = updatedEpisode.EpisodeNumber;
        existing.AirDate = updatedEpisode.AirDate;
        existing.Synopsis = updatedEpisode.Synopsis;
        existing.Duration = updatedEpisode.Duration;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var episode = await GetByIdAsync(id);
        if (episode == null) return false;

        _context.Episodes.Remove(episode);
        await _context.SaveChangesAsync();
        return true;
    }



      public async Task<bool> DeleteByAnimeIdAsync(int animeId)
    {
        var episodes = await _context.Episodes.Where(e => e.AnimeId == animeId).ToListAsync();
        if (episodes.Count == 0) return false;

        _context.Episodes.RemoveRange(episodes);
        await _context.SaveChangesAsync();
        return true;
    }


   

}
