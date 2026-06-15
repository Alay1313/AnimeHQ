

namespace Application;

public interface IFavoriteService
{
    Task<FavoriteDto?> AddFavoriteAsync(CreateFavoriteDto dto, CancellationToken ct = default);
    Task<bool> IsFavoriteAsync(string userId, int animeId, CancellationToken ct = default);
    Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(string userId, CancellationToken ct = default);
    Task<bool> RemoveFavoriteAsync(string userId, int animeId, CancellationToken ct = default);
}