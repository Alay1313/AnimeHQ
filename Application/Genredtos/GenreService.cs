using System;
using AutoMapper;

namespace Application.Genredtos;

public class GenreService
{
    private readonly IGenreRepo _repo;
    private readonly IMapper _mapper;

    public GenreService(IGenreRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }


     public async Task<IEnumerable<GenreDto>> GetAllAsync()
    {
        var genres = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<GenreDto>>(genres);
    }


     public async Task<GenreDto?> GetByNameAsync(string name)
    {
        var genre = await _repo.GetByNameAsync(name);
        return genre == null ? null : _mapper.Map<GenreDto>(genre);
    }



    public async Task<GenreDto?> CreateAsync(GenreDto dto)
    {
        
        var existing = await _repo.GetByNameAsync(dto.Name);
        if (existing != null) return null;

        var entity = _mapper.Map<Domain.Genre>(dto);
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<GenreDto>(created);
    }



    public async Task<GenreDto?> GetByIdAsync(int id)
    {
        var genre = await _repo.GetByIdAsync(id);
        return genre == null ? null : _mapper.Map<GenreDto>(genre);
    }



      public async Task<bool> DeleteAsync(int id)
    {
        
        return await _repo.DeleteAsync(id);
    }




}
