using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
namespace Application.AnimeDtos;

public class AnimeService
{
    private readonly IAnimeRepo _repo;
    private readonly IJikanService _jikan;
    private readonly IMapper _mapper;



     public AnimeService(IAnimeRepo repo, IJikanService jikan, IMapper mapper)
    {
        _repo = repo;
        _jikan = jikan;
        _mapper = mapper;
    }



    public async Task<AnimeDto?> GetAnimeByIdAsync(int id)
    {
        var anime = await _repo.GetByIdAsync(id);
        return anime == null ? null : _mapper.Map<AnimeDto>(anime);
    }



    public async Task<IEnumerable<AnimeDto>> SearchAnimesAsync(string query, int page, int pageSize)
    {
        var animes = await _jikan.SearchAnimeAsync(query, page);

    if (animes == null || !animes.Any())
        return [];

    return _mapper.Map<IEnumerable<AnimeDto>>(animes);
    }


    public async Task<AnimeDto?> FetchAndCacheFromJikanAsync(int malId)
    {
        var existing = await _repo.GetByIdAsync(malId);
        if (existing != null) return _mapper.Map<AnimeDto>(existing);


        var jikanResponse = await _jikan.GetAnimeAsync(malId);
        if (jikanResponse?.Data == null) return null;

        var animeEntity = _mapper.Map<Domain.Anime>(jikanResponse.Data);

        var saved = await _repo.CreateAsync(animeEntity);

        return _mapper.Map<AnimeDto>(saved);

    }


     public async Task<AnimeDto?> UpdateAsync(int id, AnimeDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        
        var updated = await _repo.UpdateAsync(id, existing);
        return updated == null ? null : _mapper.Map<AnimeDto>(updated);
    }



    public async Task<bool> DeleteAsync(int id)
    {
        return await _repo.DeleteAsync(id);
    }
}
