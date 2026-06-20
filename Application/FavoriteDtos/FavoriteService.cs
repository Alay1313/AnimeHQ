using AutoMapper;
using Domain;

namespace Application;

public class FavoriteService : IFavoriteService
{
     private readonly IFavoriteRepo _repo;
    private readonly IMapper _mapper;
    private readonly IAnimeService _animeService; 

    public FavoriteService(IFavoriteRepo repo, IMapper mapper, IAnimeService animeService)
    {
        _repo = repo;
        _mapper = mapper;
        _animeService = animeService;
    }

    public async Task<FavoriteDto?> AddFavoriteAsync(CreateFavoriteDto dto, CancellationToken ct = default)
    {
        var existing = await _repo.GetByUserAndAnimeIdAsync(dto.UserId, dto.AnimeId, ct);
        if (existing != null) return null;

        // Ensure the anime exists in the local DB before adding to favorites
        await _animeService.SyncFromJikanAsync(dto.AnimeId, ct);

        var favoriteEntity = _mapper.Map<Favorite>(dto);
        favoriteEntity.CreatedAt = DateTime.UtcNow;

        var created = await _repo.CreateAsync(favoriteEntity, ct);
        return _mapper.Map<FavoriteDto>(created);
       
    }

    public async Task<bool> IsFavoriteAsync(string userId, int animeId, CancellationToken ct = default)
    {
        var favorite = await _repo.GetByUserAndAnimeIdAsync(userId, animeId, ct);
        return favorite != null;
    }

    public async Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(string userId, CancellationToken ct = default)
    {
        var entities = await _repo.GetByUserIdAsync(userId, ct);
        return _mapper.Map<IEnumerable<FavoriteDto>>(entities);
    }

    public async Task<bool> RemoveFavoriteAsync(string userId, int animeId, CancellationToken ct = default)
    {
        var favorite = await _repo.GetByUserAndAnimeIdAsync(userId, animeId, ct);
        if (favorite == null) return false;

        return await _repo.DeleteAsync(favorite.Id, ct);
    }
}
