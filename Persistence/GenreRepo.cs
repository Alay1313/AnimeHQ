using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using Application;

namespace Persistence;

public class GenreRepo : IGenreRepo
{
    private readonly AppDbContext _context;

    public GenreRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Genre?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Genres
            .Include(g => g.AnimeGenres) // Needed to calculate AnimeCount
            .FirstOrDefaultAsync(g => g.Id == id, ct);
    }

    // GenreRepo.cs — implementation
    public async Task<List<int>> GetIdsByNamesAsync(List<string> names, CancellationToken ct = default)
    {
        if (names == null || names.Count == 0) return new List<int>();

        var lowerNames = names.Select(n => n.ToLower()).ToList();
        return await _context.Genres
            .Where(g => lowerNames.Contains(g.Name.ToLower()))
            .Select(g => g.Id)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Genre>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Genres
            .Include(g => g.AnimeGenres)
            .ToListAsync(ct);
    }

    public async Task<Genre?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Genres
            .FirstOrDefaultAsync(g => g.Name == name, ct);
    }

    public async Task<Genre> CreateAsync(Genre genre, CancellationToken ct = default)
    {
        var e = await _context.Genres.AddAsync(genre, ct);
        await _context.SaveChangesAsync(ct);
        return e.Entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var genre = await GetByIdAsync(id, ct);
        if (genre == null) return false;
        
        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
