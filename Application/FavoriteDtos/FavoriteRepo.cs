using System;
using Microsoft.EntityFrameworkCore;
using Domain;
using Persistence;

namespace Application;

public class FavoriteRepo(AppDbContext context) : IFavoriteRepo
{
    private readonly AppDbContext _context = context;
    

     public async Task<Favorite?> GetByIdAsync(int id)
    {
        return await _context.Favorites
            .Include(f => f.User)
            .Include(f => f.Anime)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IEnumerable<Favorite>> GetByUserIdAsync(string userId)
    {
         return await _context.Favorites
            .Include(f => f.User)
            .Include(f => f.Anime)
            .Where(f => f.UserId == userId)
            .ToListAsync();
    }


      public async Task<Favorite?> GetByUserAndAnimeIdAsync(string userId, int animeId)
    {
        return await _context.Favorites
            .Include(f => f.User)
            .Include(f => f.Anime)
            .FirstOrDefaultAsync(f => f.UserId == userId && f.AnimeId == animeId);
    }
       

   
    public async Task<Favorite> CreateAsync(Favorite favorite)
    {
        var entity = await _context.Favorites.AddAsync(favorite);
        await _context.SaveChangesAsync();
        return entity.Entity;
    }
        



   public async Task<bool> DeleteAsync(int id)
    {
        var favorite = await GetByIdAsync(id);
        if (favorite == null) return false;

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }

   
}
