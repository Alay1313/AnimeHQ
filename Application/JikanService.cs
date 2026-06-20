using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Polly.Retry;
using Polly;
using System.Net;
using System.Text.Json;

namespace Application;

public class JikanService : IJikanService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<JikanService> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly SemaphoreSlim _throttle = new SemaphoreSlim(1, 1);

    public JikanService(HttpClient httpClient, ILogger<JikanService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => r.StatusCode == (HttpStatusCode)429)
            .WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(1),
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
            await _throttle.WaitAsync();
            try
            {
                await Task.Delay(400);
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"anime/{malId}"));

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<JikanAnimeResponse>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("Jikan GetAnime {MalId}: data null={IsNull}",
                    malId, result?.Data == null);

                return result;
            }
            finally
            {
                _throttle.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch anime {MalId} from Jikan", malId);
            return null;
        }
    }

    public async Task<IEnumerable<JikanAnimeData>> SearchAnimeAsync(string query, int page = 1)
    {
        var url = $"anime?q={Uri.EscapeDataString(query)}&page={page}&limit=20";
        var response = await FetchJikanDataAsync<JikanApiResponse<JikanAnimeData>>(url, default);
        return response?.Data ?? Enumerable.Empty<JikanAnimeData>();
    }

    public async Task<JikanEpisodeResponse?> GetEpisodesAsync(int malId, int page = 1)
    {
        return await FetchJikanDataAsync<JikanEpisodeResponse>($"anime/{malId}/episodes?page={page}", default);
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

    public async Task<JikanMangaResponse?> GetMangaAsync(int malId)
    {
        try
        {
            await _throttle.WaitAsync();
            try
            {
                await Task.Delay(400);
                var response = await _retryPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"manga/{malId}"));

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<JikanMangaResponse>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            finally
            {
                _throttle.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch manga {MalId} from Jikan", malId);
            return null;
        }
    }

    public async Task<IEnumerable<JikanMangaData>> SearchMangaAsync(string query, int page = 1)
    {
        var url = $"manga?q={Uri.EscapeDataString(query)}&page={page}&limit=20";
        var response = await FetchJikanDataAsync<JikanMangaApiResponse<JikanMangaData>>(url, default);
        return response?.Data ?? Enumerable.Empty<JikanMangaData>();
    }

    public async Task<IEnumerable<JikanMangaData>> GetTopMangaAsync(int page = 1, int pageSize = 25, CancellationToken ct = default)
    {
        var url = $"top/manga?page={page}&limit={pageSize}";
        var response = await FetchJikanDataAsync<JikanMangaApiResponse<JikanMangaData>>(url, ct);
        return response?.Data ?? Enumerable.Empty<JikanMangaData>();
    }

    private async Task<T?> FetchJikanDataAsync<T>(string url, CancellationToken ct)
    {
        await _throttle.WaitAsync(ct);
        try
        {
            await Task.Delay(400, ct);

            var response = await _retryPolicy.ExecuteAsync(ct2 =>
                _httpClient.GetAsync(url, ct2), ct);

            if (response.StatusCode == HttpStatusCode.TooManyRequests ||
                response.StatusCode == HttpStatusCode.GatewayTimeout ||
                response.StatusCode == HttpStatusCode.BadGateway)
            {
                _logger.LogWarning("[JikanService] Jikan API returned {StatusCode} for {Url}. Falling back to local cache.",
                    (int)response.StatusCode, url);
                return default;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[JikanService] Failed to fetch data from {Url}", url);
            return default;
        }
        finally
        {
            _throttle.Release();
        }
    }
}
   



 




