using AutoMapper;
using Domain;
using Microsoft.Extensions.Caching.Memory;

namespace Application;

public class MangaService : IMangaService
{
    private readonly IMangaRepo _repo;
    private readonly IMangaFavoriteRepo _favoriteRepo;
    private readonly IMangaReviewRepo _reviewRepo;
    private readonly IJikanService _jikan;
    private readonly IMapper _mapper;
     private readonly IMemoryCache _cache;

    public MangaService(IMangaRepo repo, IMangaFavoriteRepo favoriteRepo,
        IMangaReviewRepo reviewRepo, IJikanService jikan, IMapper mapper, IMemoryCache cache)
    {
        _repo = repo;
        _favoriteRepo = favoriteRepo;
        _reviewRepo = reviewRepo;
        _jikan = jikan;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<MangaDto> GetByIdAsync(int mangaId, CancellationToken ct = default)
    {
        var manga = await _repo.GetByIdAsync(mangaId, ct);
        if (manga == null)
            return await SyncFromJikanAsync(mangaId, ct);
        return _mapper.Map<MangaDto>(manga);
    }

    public async Task<IEnumerable<MangaDto>> SearchAsync(string query, int page = 1, int pageSize = 25, CancellationToken ct = default)
    {
        var cacheKey = $"MangaSearch_{query}_{page}_{pageSize}";
        
         // 1. Return from cache if it exists
        if (_cache.TryGetValue(cacheKey, out IEnumerable<MangaDto>? cachedMangas))
        {
            return cachedMangas!;
        }

         var results = await _jikan.SearchMangaAsync(query, page);
        IEnumerable<MangaDto> mappedResults;
        
        // FALLBACK: If Jikan is down/rate-limited (returns null), fetch from local DB cache
        if (results == null)
        {
            var cached = await _repo.SearchAsync(query, page, pageSize, ct);
            mappedResults =  _mapper.Map<IEnumerable<MangaDto>>(cached);
        }
        else
        {
            mappedResults = _mapper.Map<IEnumerable<MangaDto>>(results);
        }

        // 4. Save to cache for 30 minutes
        _cache.Set(cacheKey, mappedResults, TimeSpan.FromMinutes(30));
        return mappedResults;
        
        
    }

    public async Task<IEnumerable<MangaDto>> GetTopMangaAsync(int page = 1, int pageSize = 25, CancellationToken ct = default)
    {
        var cacheKey = $"TopManga_{page}_{pageSize}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<MangaDto>? cachedMangas))
        {
            return cachedMangas!;
        }

        var results = await _jikan.GetTopMangaAsync(page, pageSize, ct);
        IEnumerable<MangaDto> mappedResults;
        
        // FALLBACK: If Jikan is down/rate-limited, fetch from local DB cache
        if (results == null)
        {
            var cached = await _repo.GetTopCachedAsync(page, pageSize, ct);
            mappedResults =  _mapper.Map<IEnumerable<MangaDto>>(cached);
        }
        else
        {
            mappedResults = _mapper.Map<IEnumerable<MangaDto>>(results);
        }

         // Save to cache for 1 hour
        _cache.Set(cacheKey, mappedResults, TimeSpan.FromHours(1));
        return mappedResults;
        
        
    }

    public async Task<MangaDto> SyncFromJikanAsync(int mangaId, CancellationToken ct = default)
    {
        var response = await _jikan.GetMangaAsync(mangaId);
        if (response?.Data == null) throw new Exception("Failed to fetch manga from Jikan API.");

        var existing = await _repo.GetByIdAsync(mangaId, ct);
        var manga = _mapper.Map<Manga>(response.Data);
        manga.CachedAt = DateTime.UtcNow;

        if (existing == null)
        {
            var created = await _repo.CreateAsync(manga, ct);
            return _mapper.Map<MangaDto>(created);
        }

        manga.MangaId = mangaId;
        var updated = await _repo.UpdateAsync(mangaId, manga, ct);
        return _mapper.Map<MangaDto>(updated);
    }

    public async Task<MangaFavoriteDto?> AddFavoriteAsync(CreateMangaFavoriteDto dto, CancellationToken ct = default)
    {
        var existing = await _favoriteRepo.GetByUserAndMangaIdAsync(dto.UserId, dto.MangaId, ct);
        if (existing != null) return null;

        await SyncFromJikanAsync(dto.MangaId, ct);

        var favorite = _mapper.Map<MangaFavorite>(dto);
        favorite.CreatedAt = DateTime.UtcNow;
        var created = await _favoriteRepo.CreateAsync(favorite, ct);
        return _mapper.Map<MangaFavoriteDto>(created);
    }

    public async Task<bool> RemoveFavoriteAsync(string userId, int mangaId, CancellationToken ct = default)
    {
        var favorite = await _favoriteRepo.GetByUserAndMangaIdAsync(userId, mangaId, ct);
        if (favorite == null) return false;
        return await _favoriteRepo.DeleteAsync(favorite.Id, ct);
    }

    public async Task<bool> IsFavoriteAsync(string userId, int mangaId, CancellationToken ct = default)
    {
        var favorite = await _favoriteRepo.GetByUserAndMangaIdAsync(userId, mangaId, ct);
        return favorite != null;
    }

    public async Task<MangaReviewDto?> CreateReviewAsync(CreateMangaReviewDto dto, CancellationToken ct = default)
    {
        await SyncFromJikanAsync(dto.MangaId, ct);
        var review = _mapper.Map<MangaReview>(dto);
        review.CreatedAt = DateTime.UtcNow;
        var created = await _reviewRepo.CreateAsync(review, ct);
        return _mapper.Map<MangaReviewDto>(created);
    }

    public async Task<IEnumerable<MangaReviewDto>> GetReviewsAsync(int mangaId, CancellationToken ct = default)
    {
        var reviews = await _reviewRepo.GetByMangaIdAsync(mangaId, ct);
        return _mapper.Map<IEnumerable<MangaReviewDto>>(reviews);
    }

    public async Task<bool> DeleteReviewAsync(int reviewId, CancellationToken ct = default)
    {
        return await _reviewRepo.DeleteAsync(reviewId, ct);
    }



    public async Task<IEnumerable<MangaFavoriteDto>> GetUserFavoritesAsync(string userId, CancellationToken ct = default)
    {
        var favorites = await _favoriteRepo.GetByUserIdAsync(userId, ct);
        return _mapper.Map<IEnumerable<MangaFavoriteDto>>(favorites);
    }

    public async Task<IEnumerable<MangaReviewDto>> GetUserReviewsAsync(string userId, CancellationToken ct = default)
    {
        var reviews = await _reviewRepo.GetByUserIdAsync(userId, ct); 
        return _mapper.Map<IEnumerable<MangaReviewDto>>(reviews);
    }


}