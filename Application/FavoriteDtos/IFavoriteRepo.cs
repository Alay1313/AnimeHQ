using System;
using Domain;
namespace Application;

public interface IFavoriteRepo
{
    Task<Favorite?> GetByIdAsync(int id);
    Task<IEnumerable<Favorite>> GetByUserIdAsync(string userId);
    Task<Favorite?> GetByUserAndAnimeIdAsync(string userId, int animeId);
    Task<Favorite> CreateAsync(Favorite favorite);
    Task<bool> DeleteAsync(int id);

}
