using System;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
namespace Application;

public class GenreRepo(AppDbContext context) : IGenreRepo
{
     private readonly AppDbContext _context = context;

        
    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _context.Genres.Include(g => g.AnimeGenres).FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return (IEnumerable<User>)await _context.AnimeGenres.ToListAsync();
    }

    public async Task<Genre?> GetByNameAsync(string name)
    {
        return await _context.Genres.FirstOrDefaultAsync(g => g.Name == name);
    }

    public async Task<Genre> CreateASync(Genre genre)
    {
       var e = await _context.Genres.AddAsync(genre); 
       await _context.SaveChangesAsync(); 
       return e.Entity; 
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var g = await GetByIdAsync(id); 
        if (g == null) return false;
        _context.Genres.Remove(g);
        await _context.SaveChangesAsync();
        return true;
        
    }

    Task<IEnumerable<Genre>> IGenreRepo.GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Genre> CreateAsync(Genre genre)
    {
        throw new NotImplementedException();
    }
}
