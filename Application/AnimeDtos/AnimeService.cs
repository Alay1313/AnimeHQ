using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Domain;


namespace Application;

public class AnimeService : IAnimeService
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

    public async Task<AnimeDto> GetByIdAsync(int animeListId, CancellationToken ct = default)
    {
        var anime = await _repo.GetByIdAsync(animeListId, ct);
        
         if (anime == null) 
        {
            return await SyncFromJikanAsync(animeListId, ct);
        }
        
        return _mapper.Map<AnimeDto>(anime);
    }

    public async Task<IEnumerable<AnimeDto>> SearchAsync(string query, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var localResults = await _repo.SearchAsync(query, page, pageSize, ct);
    
        if (localResults.Any())
            return _mapper.Map<IEnumerable<AnimeDto>>(localResults); 
         
         
         var jikanResults = await _jikan.SearchAnimeAsync(query, page);
         return _mapper.Map<IEnumerable<AnimeDto>>(jikanResults);
        
    }

    public async Task<AnimeDto> CreateAsync(CreateAnimeDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Anime>(dto);
        var created = await _repo.CreateAsync(entity, ct);
        return _mapper.Map<AnimeDto>(created);
    }

    public async Task<AnimeDto> UpdateAsync(int animeListId, UpdateAnimeDto dto, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(animeListId, ct) ?? throw new KeyNotFoundException($"Anime {animeListId} not found.");

        // 1. Map scalar properties from DTO to the existing entity
        _mapper.Map(dto, existing);

        // 2. Handle collection updates manually to ensure clean replacement
        if (dto.GenreIds != null)
        {
            existing.AnimeGenres.Clear();
            foreach (var genreId in dto.GenreIds)
            {
                existing.AnimeGenres.Add(new AnimeGenre 
                { 
                    AnimeId = existing.AnimeListId, 
                    GenreId = genreId 
                });
            }
        }

        // 3. Save and return mapped DTO
        var updated = await _repo.UpdateAsync(animeListId, existing, ct);
        return _mapper.Map<AnimeDto>(updated);
    }

    public async Task<bool> DeleteAsync(int animeListId, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(animeListId, ct);
    }

    public async Task<AnimeDto> SyncFromJikanAsync(int animeListId, CancellationToken ct = default)
    {
        var response = await _jikan.GetAnimeAsync(animeListId);
        if (response?.Data == null) throw new Exception("Failed to fetch data from Jikan API.");

        var data = response.Data;
        
        // Map Jikan data directly to UpdateAnimeDto using our new profile mapping
        var updateDto = _mapper.Map<UpdateAnimeDto>(data);

        var existing = await _repo.GetByIdAsync(animeListId, ct);
        
        if (existing == null)
        {
            // If it doesn't exist locally, map to CreateAnimeDto and create it
            var createDto = _mapper.Map<CreateAnimeDto>(updateDto);
            createDto.AnimeListId = animeListId; // Ensure the ID is set for creation
            return await CreateAsync(createDto, ct);
        }
        
        return await UpdateAsync(animeListId, updateDto, ct);
    }

    public async Task<IEnumerable<AnimeDto>> GetTopAiringAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
    {
        var jikanData = await _jikan.GetTopAiringAsync(page, pageSize, type, ct);
        return _mapper.Map<IEnumerable<AnimeDto>>(jikanData);
    }


    public async Task<IEnumerable<AnimeDto>> GetPopularAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
    {
        var jikanData = await _jikan.GetPopularAsync(page, pageSize, type, ct);
        return _mapper.Map<IEnumerable<AnimeDto>>(jikanData);
    }



  public async Task<IEnumerable<AnimeDto>> GetSeasonalAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
  {
        var jikanData = await _jikan.GetSeasonalAsync(page, pageSize, type, ct);
        return _mapper.Map<IEnumerable<AnimeDto>>(jikanData);
  }



   public async Task<IEnumerable<AnimeDto>> GetUpcomingAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
    {
        var jikanData = await _jikan.GetUpcomingAsync(page, pageSize, type, ct);
        return _mapper.Map<IEnumerable<AnimeDto>>(jikanData);
    }

}
