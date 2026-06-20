using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;

namespace Application;

public interface IAnimeRepo
{
    Task<Anime?> GetByIdAsync(int animeListId, CancellationToken ct = default);
    Task<IEnumerable<Anime>> SearchAsync(string query, int page, int pageSize, List<int>? genreIds = null, CancellationToken ct = default);
    Task<Anime> CreateAsync(Anime anime, CancellationToken ct = default);
    Task<Anime> UpdateAsync(int animeListId, Anime anime, CancellationToken ct = default);
    Task<bool> DeleteAsync(int animeListId, CancellationToken ct = default);
}