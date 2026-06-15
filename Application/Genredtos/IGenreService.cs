

namespace Application;

public interface IGenreService
{
    Task<IEnumerable<GenreDto>> GetAllAsync(CancellationToken ct = default);
    Task<GenreDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<GenreDto?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<GenreDto?> CreateAsync(CreateGenreDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}