using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Application;

public interface IFavoriteRepo
{
    Task<Favorite?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Favorite>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<Favorite?> GetByUserAndAnimeIdAsync(string userId, int animeId, CancellationToken ct = default);
    Task<Favorite> CreateAsync(Favorite favorite, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}