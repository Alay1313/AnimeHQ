using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain; // Adjust if your DTOs are in a different namespace like 'Application'

namespace Application;

public interface IAnimeService
{

    // Retrieves a single anime by its ID
    Task<AnimeDto> GetByIdAsync(int animeListId, CancellationToken ct = default);

    // Searches for anime by title with pagination
    Task<IEnumerable<AnimeDto>> SearchAsync(string query, int page = 1, int pageSize = 20, CancellationToken ct = default);

    
    //Creates a new anime entry in the database
    Task<AnimeDto> CreateAsync(CreateAnimeDto dto, CancellationToken ct = default);

    
    //Updates an existing anime entry. Returns the updated AnimeDto
    Task<AnimeDto> UpdateAsync(int animeListId, UpdateAnimeDto dto, CancellationToken ct = default);


    //Deletes an anime entry by its ID
    Task<bool> DeleteAsync(int animeListId, CancellationToken ct = default);

    
    // Fetches the latest data for an anime from the Jikan API and syncs it to the local database
    Task<AnimeDto> SyncFromJikanAsync(int animeListId, CancellationToken ct = default);


    //Retrieves the top airing anime directly from the Jikan API
    Task<IEnumerable<AnimeDto>> GetTopAiringAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default);


     
    // Retrieves the most popular anime directly from the Jikan API
    Task<IEnumerable<AnimeDto>> GetPopularAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default);




    // Retrieves the current season's anime directly from the Jikan API
    Task<IEnumerable<AnimeDto>> GetSeasonalAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default);


    /// <summary>
    /// Retrieves upcoming anime directly from the Jikan API, optionally filtered by type.
    /// </summary>
    Task<IEnumerable<AnimeDto>> GetUpcomingAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default);
}