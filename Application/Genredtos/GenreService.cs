using AutoMapper;
using Domain;
using Persistence;

namespace Application;

public class GenreService : IGenreService
{
    private readonly IGenreRepo _repo;
    private readonly IMapper _mapper;

    public GenreService(IGenreRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GenreDto>> GetAllAsync(CancellationToken ct = default)
    {
        var genres = await _repo.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<GenreDto>>(genres);
    }

    public async Task<GenreDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var genre = await _repo.GetByIdAsync(id, ct);
        return genre == null ? null : _mapper.Map<GenreDto>(genre);
    }

    public async Task<GenreDto?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var genre = await _repo.GetByNameAsync(name, ct);
        return genre == null ? null : _mapper.Map<GenreDto>(genre);
    }

    public async Task<GenreDto?> CreateAsync(CreateGenreDto dto, CancellationToken ct = default)
    {
        var existing = await _repo.GetByNameAsync(dto.Name, ct);
        if (existing != null) 
        {
            return null; // Genre already exists
        }

        var entity = _mapper.Map<Genre>(dto);
        var created = await _repo.CreateAsync(entity, ct);
        return _mapper.Map<GenreDto>(created);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(id, ct);
    }
}
