using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using Domain;
using Persistence;

namespace Application;

public class AnimeService : IAnimeService
{
    private readonly IAnimeRepo _repo;
    private readonly IJikanService _jikan;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly IGenreRepo _genreRepo;

    public AnimeService(IAnimeRepo repo, IJikanService jikan, IMapper mapper, IMemoryCache cache, IGenreRepo genreRepo)
    {
        _repo = repo;
        _jikan = jikan;
        _mapper = mapper;
        _cache = cache;
        _genreRepo = genreRepo;
    }

    public async Task<AnimeDto> GetByIdAsync(int animeListId, CancellationToken ct = default)
    {
        var anime = await _repo.GetByIdAsync(animeListId, ct);
        if (anime != null)
            return _mapper.Map<AnimeDto>(anime);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(8));

        try
        {
            return await SyncFromJikanAsync(animeListId, cts.Token);
        }
        catch (OperationCanceledException)
        {
            throw new Exception($"Timed out fetching anime {animeListId} from Jikan.");
        }
    }

    public async Task<IEnumerable<AnimeDto>> SearchAsync(string query, int page = 1, int pageSize = 20, List<int>? genreIds = null, CancellationToken ct = default)
    {
        var cacheKey = $"AnimeSearch_{query}_{page}_{pageSize}_{(genreIds != null ? string.Join(",", genreIds) : "none")}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<AnimeDto>? cachedSearch))
            return cachedSearch!;

        var localResults = await _repo.SearchAsync(query, page, pageSize, genreIds, ct);
        IEnumerable<AnimeDto> mappedResults;

        if (localResults.Any())
        {
            mappedResults = _mapper.Map<IEnumerable<AnimeDto>>(localResults);
        }
        else
        {
            // Genre filtering only applies to local DB results; Jikan fallback is unfiltered by genre
            var jikanResults = await _jikan.SearchAnimeAsync(query, page);
            mappedResults = jikanResults == null
                ? Enumerable.Empty<AnimeDto>()
                : _mapper.Map<IEnumerable<AnimeDto>>(jikanResults);
        }

        _cache.Set(cacheKey, mappedResults, TimeSpan.FromMinutes(30));
        return mappedResults;
    }

    public async Task<AnimeDto> CreateAsync(CreateAnimeDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Anime>(dto);
        var created = await _repo.CreateAsync(entity, ct);
        return _mapper.Map<AnimeDto>(created);
    }

    public async Task<AnimeDto> UpdateAsync(int animeListId, UpdateAnimeDto dto, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(animeListId, ct)
            ?? throw new KeyNotFoundException($"Anime {animeListId} not found.");

        _mapper.Map(dto, existing);

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
        var updateDto = _mapper.Map<UpdateAnimeDto>(data);

        // Map Jikan genre names to local Genre IDs 
        var jikanGenreNames = data.Genres?.Select(g => g.Name).ToList() ?? new List<string>();
        var matchedGenreIds = await _genreRepo.GetIdsByNamesAsync(jikanGenreNames, ct);
        updateDto.GenreIds = matchedGenreIds;

        var existing = await _repo.GetByIdAsync(animeListId, ct);

        if (existing == null)
        {
            var createDto = _mapper.Map<CreateAnimeDto>(updateDto);
            createDto.AnimeListId = animeListId;
            return await CreateAsync(createDto, ct);
        }

        return await UpdateAsync(animeListId, updateDto, ct);
    }

    public async Task<IEnumerable<AnimeDto>> GetTopAiringAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
    {
        var cacheKey = $"TopAiring_{page}_{pageSize}_{type}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<AnimeDto>? cachedAnimes))
            return cachedAnimes!;

        var jikanData = await _jikan.GetTopAiringAsync(page, pageSize, type, ct);
        IEnumerable<AnimeDto> result;

        if (jikanData == null || !jikanData.Any())
        {
            var dbFallback = await _repo.SearchAsync(string.Empty, page, pageSize, null, ct);
            result = _mapper.Map<IEnumerable<AnimeDto>>(dbFallback);
        }
        else
        {
            result = _mapper.Map<IEnumerable<AnimeDto>>(jikanData);
        }

        _cache.Set(cacheKey, result, TimeSpan.FromHours(1));
        return result;
    }

    public async Task<IEnumerable<AnimeDto>> GetPopularAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
    {
        var cacheKey = $"Popular_{page}_{pageSize}_{type}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<AnimeDto>? cachedAnimes))
            return cachedAnimes!;

        var jikanData = await _jikan.GetPopularAsync(page, pageSize, type, ct);
        IEnumerable<AnimeDto> result;

        if (jikanData == null || !jikanData.Any())
        {
            var cached = await _repo.SearchAsync(string.Empty, page, pageSize, null, ct);
            result = _mapper.Map<IEnumerable<AnimeDto>>(cached);
        }
        else
        {
            result = _mapper.Map<IEnumerable<AnimeDto>>(jikanData);
        }

        _cache.Set(cacheKey, result, TimeSpan.FromHours(1));
        return result;
    }

    public async Task<IEnumerable<AnimeDto>> GetSeasonalAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
    {
        var cacheKey = $"Seasonal_{page}_{pageSize}_{type}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<AnimeDto>? cachedAnimes))
            return cachedAnimes!;

        var jikanData = await _jikan.GetSeasonalAsync(page, pageSize, type, ct);
        IEnumerable<AnimeDto> result;

        if (jikanData == null || !jikanData.Any())
        {
            var cached = await _repo.SearchAsync(string.Empty, page, pageSize, null, ct);
            result = _mapper.Map<IEnumerable<AnimeDto>>(cached);
        }
        else
        {
            result = _mapper.Map<IEnumerable<AnimeDto>>(jikanData);
        }

        _cache.Set(cacheKey, result, TimeSpan.FromHours(1));
        return result;
    }

    public async Task<IEnumerable<AnimeDto>> GetUpcomingAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
    {
        var cacheKey = $"Upcoming_{page}_{pageSize}_{type}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<AnimeDto>? cachedAnimes))
            return cachedAnimes!;

        var jikanData = await _jikan.GetUpcomingAsync(page, pageSize, type, ct);
        IEnumerable<AnimeDto> result;

        if (jikanData == null || !jikanData.Any())
        {
            var cached = await _repo.SearchAsync(string.Empty, page, pageSize, null, ct);
            result = _mapper.Map<IEnumerable<AnimeDto>>(cached);
        }
        else
        {
            result = _mapper.Map<IEnumerable<AnimeDto>>(jikanData);
        }

        _cache.Set(cacheKey, result, TimeSpan.FromHours(1));
        return result;
    }
}