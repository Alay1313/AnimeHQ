

using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;
using Polly;
using System.Net;



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
            return await _httpClient.GetFromJsonAsync<JikanAnimeResponse>($"anime/{malId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch anime {MalId} from Jikan", malId);
            return null;
        }
    }

    





    public async Task<IEnumerable<JikanAnimeData>> SearchAnimeAsync(string query, int page = 1)
{
    var escaped = Uri.EscapeDataString(query);

    var response = await _httpClient.GetFromJsonAsync<JikanSearchResponse>(
        $"anime?q={escaped}&page={page}&limit=20"
    );

    Console.WriteLine("DEBUG COUNT: " + response?.Data?.Count);

    return response?.Data ?? new List<JikanAnimeData>();
}




    public async Task<JikanEpisodeResponse?> GetEpisodesAsync(int malId, int page = 1)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<JikanEpisodeResponse>($"anime/{malId}/episodes?page={page}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch episodes for anime {MalId} from Jikan", malId);
            return null;
        }
    }









    














    


}
  
    
   



 




