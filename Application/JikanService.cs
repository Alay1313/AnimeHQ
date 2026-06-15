

using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
//using Microsoft.EntityFrameworkCore;
using Polly.Retry;
using Polly;
using System.Net;
using System.Text.Json;



namespace Application;

public class JikanService: IJikanService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<JikanService> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    

    public JikanService(HttpClient httpClient, ILogger<JikanService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;


        _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => r.StatusCode == (HttpStatusCode)429)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning("Jikan API retry {RetryCount} after {TimeSpan}s",
                        retryCount, timeSpan.TotalSeconds);
                });
        
    
   
    }


    public async Task<JikanAnimeResponse?> GetAnimeAsync(int malId)
    {


        try
    {
        var response = await _retryPolicy.ExecuteAsync(() => 
            _httpClient.GetAsync($"anime/{malId}"));

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JikanAnimeResponse>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Temporary: log what we actually got back
        _logger.LogInformation("Jikan GetAnime {MalId}: data null={IsNull}", 
            malId, result?.Data == null);

        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to fetch anime {MalId} from Jikan", malId);
        return null;
    }
        // try
        // {
        //     return await _httpClient.GetFromJsonAsync<JikanAnimeResponse>($"anime/{malId}");
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Failed to fetch anime {MalId} from Jikan", malId);
        //     return null;
        // }
    }

    





    public async Task<IEnumerable<JikanAnimeData>> SearchAnimeAsync(string query, int page = 1)
{
    
    var url = $"anime?q={Uri.EscapeDataString(query)}&page={page}&limit=20";
    var response = await FetchJikanDataAsync<JikanApiResponse<JikanAnimeData>>(url, default);
    return response?.Data ?? Enumerable.Empty<JikanAnimeData>();
    
    
    
    
    
    
    
    // var escaped = Uri.EscapeDataString(query);

    // var response = await _httpClient.GetFromJsonAsync<JikanSearchResponse>(
    //     $"anime?q={escaped}&page={page}&limit=20"
    // );

    // Console.WriteLine("DEBUG COUNT: " + response?.Data?.Count);

    // return response?.Data ?? new List<JikanAnimeData>();
}




    public async Task<JikanEpisodeResponse?> GetEpisodesAsync(int malId, int page = 1)
    {
        
        return await FetchJikanDataAsync<JikanEpisodeResponse>($"anime/{malId}/episodes?page={page}", default);
        
        
        
        // try
        // {
        //     return await _httpClient.GetFromJsonAsync<JikanEpisodeResponse>($"anime/{malId}/episodes?page={page}");
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Failed to fetch episodes for anime {MalId} from Jikan", malId);
        //     return null;
        // }
    }



   public async Task<IEnumerable<JikanAnimeData>> GetTopAiringAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
    {
        var url = $"top/anime?filter=airing&page={page}&limit={pageSize}";
        if (!string.IsNullOrWhiteSpace(type) && !type.Equals("all", StringComparison.OrdinalIgnoreCase))
            url += $"&type={type.ToLower()}";
        
        var response = await FetchJikanDataAsync<JikanApiResponse<JikanAnimeData>>(url, ct);
        return response?.Data ?? Enumerable.Empty<JikanAnimeData>();
    }



   public async Task<IEnumerable<JikanAnimeData>> GetPopularAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
   {
        var url = $"top/anime?filter=bypopularity&page={page}&limit={pageSize}";
        if (!string.IsNullOrWhiteSpace(type) && !type.Equals("all", StringComparison.OrdinalIgnoreCase))
            url += $"&type={type.ToLower()}";
        
        var response = await FetchJikanDataAsync<JikanApiResponse<JikanAnimeData>>(url, ct);
        return response?.Data ?? Enumerable.Empty<JikanAnimeData>();
   }





   public async Task<IEnumerable<JikanAnimeData>> GetSeasonalAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
   {
        var url = $"seasons/now?page={page}&limit={pageSize}";
        if (!string.IsNullOrWhiteSpace(type) && !type.Equals("all", StringComparison.OrdinalIgnoreCase))
            url += $"&type={type.ToLower()}";
        
        var response = await FetchJikanDataAsync<JikanApiResponse<JikanAnimeData>>(url, ct);
        return response?.Data ?? Enumerable.Empty<JikanAnimeData>();
   }



    public async Task<IEnumerable<JikanAnimeData>> GetUpcomingAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default)
    {
        
         var url = $"seasons/upcoming?page={page}&limit={pageSize}";
         if (!string.IsNullOrWhiteSpace(type) && !type.Equals("all", StringComparison.OrdinalIgnoreCase))
                url += $"&type={type.ToLower()}";

        var response = await FetchJikanDataAsync<JikanApiResponse<JikanAnimeData>>(url, ct);
        return response?.Data ?? Enumerable.Empty<JikanAnimeData>();
        
        
        
    }



    private async Task<T?> FetchJikanDataAsync<T>(string url, CancellationToken ct)
    {
        var response = await _retryPolicy.ExecuteAsync(ct2 => 
        _httpClient.GetAsync(url, ct2), ct);
    
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
        cancellationToken: ct);
        
        
    }








    














    


}
  
    
   



 




