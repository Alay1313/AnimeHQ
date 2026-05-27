
using Domain;
using Persistence;
using Microsoft.EntityFrameworkCore;
namespace Application;

public class AnimeRepo : IAnimeRepo
{
    private readonly AppDbContext _context;
    public AnimeRepo(AppDbContext context)
    {
        _context = context;
    }


    public async Task<Anime?> GetByIdAsync(int AnimeListId)
    {
        return await _context.Animes
            .Include(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
            .Include(a => a.Reviews)
            .FirstOrDefaultAsync(a => a.AnimeListId == AnimeListId);
    }


      public async Task<IEnumerable<Anime>> SearchAsync(string query, int page = 1, int pageSize = 20)
    {
        var queryable = _context.Animes
            .Include(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
            .AsQueryable();

        
        if (!string.IsNullOrWhiteSpace(query))
            queryable = queryable.Where(a => a.Title.Contains(query));

        return await queryable
            .OrderBy(a => a.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }



    public async Task<Anime> CreateAsync(Anime anime)
    {
        var entity = await _context.Animes.AddAsync(anime);
        await _context.SaveChangesAsync();
        return entity.Entity;
    }


    public async Task<Anime?> UpdateAsync(int AnimeListId, Anime updatedAnime)
    {
         var existing = await GetByIdAsync(AnimeListId);
        if (existing == null) return null;

        
        existing.Title = updatedAnime.Title;
        existing.ImageURL = updatedAnime.ImageURL;
        existing.Score = updatedAnime.Score;
        existing.Synopsis = updatedAnime.Synopsis;
        existing.Type = updatedAnime.Type;
        existing.Episodes = updatedAnime.Episodes;
        existing.Status = updatedAnime.Status;
        existing.AiredFrom = updatedAnime.AiredFrom;
        existing.AiredTo = updatedAnime.AiredTo;
        existing.CachedAt = updatedAnime.CachedAt;

        await _context.SaveChangesAsync();
        return existing;
    }



    public async Task<bool> DeleteAsync(int AnimeListId)
    {
        var anime = await GetByIdAsync(AnimeListId);
        if (anime == null) return false;

        _context.Animes.Remove(anime);
        await _context.SaveChangesAsync();
        return true;
    }
}
