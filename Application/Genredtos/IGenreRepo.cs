using System;
using Domain;

namespace Persistence;

public interface IGenreRepo
{
    Task<Genre?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Genre>> GetAllAsync(CancellationToken ct = default);
    Task<Genre?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<Genre> CreateAsync(Genre genre, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
