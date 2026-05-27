using System;
using AutoMapper;
using Domain;
namespace Application;

public class FavoriteService(IFavoriteRepo repo, IMapper mapper)
{
    private readonly IFavoriteRepo _repo = repo;
    private readonly IMapper _mapper = mapper;

    public async Task<FavoriteDto?> AddFavoriteAsync(CreateFavoriteDto dto)
    {
        var existing = await _repo.GetByUserAndAnimeIdAsync(dto.UserId, dto.AnimeId);
        if (existing != null) return null;

        var favoriteEntity = _mapper.Map<Favorite>(dto);

        var created = await _repo.CreateAsync(favoriteEntity);

        return _mapper.Map<FavoriteDto>(created);
    }

     public async Task<bool> IsFavoriteAsync(string userId, int animeId)
    {
        var favorite = await _repo.GetByUserAndAnimeIdAsync(userId, animeId);
        return favorite != null;
    }



     public async Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(string userId)
    {
        var entities = await _repo.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<FavoriteDto>>(entities);
    }

    

     public async Task<bool> RemoveFavoriteAsync(string userId, int animeId)
    {
        var favorite = await _repo.GetByUserAndAnimeIdAsync(userId, animeId);
        if (favorite == null) return false;

        return await _repo.DeleteAsync(favorite.Id);
    }
}
