using System;
using System.Text.Json.Serialization;
using JikanDotNet;


namespace Application;

public interface IJikanService
{
    Task<JikanAnimeResponse?> GetAnimeAsync(int malId);
    Task<IEnumerable<JikanAnimeData>> SearchAnimeAsync(string query, int page = 1);
    
    Task<JikanEpisodeResponse?> GetEpisodesAsync(int malId, int page = 1);

    Task<IEnumerable<JikanAnimeData>> GetTopAiringAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default);

    Task<IEnumerable<JikanAnimeData>> GetPopularAsync(int page = 1, int pageSize = 25, string? type = null,  CancellationToken ct = default);


    Task<IEnumerable<JikanAnimeData>> GetSeasonalAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default);

    Task<IEnumerable<JikanAnimeData>> GetUpcomingAsync(int page = 1, int pageSize = 25, string? type = null, CancellationToken ct = default);
    
    // IJikanService additions
    Task<JikanMangaResponse?> GetMangaAsync(int malId);
    Task<IEnumerable<JikanMangaData>> SearchMangaAsync(string query, int page = 1);
    Task<IEnumerable<JikanMangaData>> GetTopMangaAsync(int page = 1, int pageSize = 25, CancellationToken ct = default);


}




//DTos for jikan

public class JikanApiResponse<T>
{
    public List<T> Data { get; set; } = new();
    public Pagination Pagination { get; set; } = new();
}




public class JikanAnimeResponse
{
    public JikanAnimeData? Data { get; set; }
}

    public class JikanSearchItem
{
    public int MalId { get; set; }
    public string? Title { get; set; }
     public JikanImages? Images { get; set; }
}


public class JikanEpisodeResponse
 { 
 public List<JikanEpisodeData> Data { get; set; } = new();  
 public JikanPagination? Pagination { get; set; }
  }




public class JikanEpisodeData
{
public int MalId { get; set; } 
 public int EpisodeNumber { get; set; } 
 public string? Title { get; set; }
public string? TitleJapanese { get; set; } 
 public DateTime? Aired { get; set; } 
 public string? Synopsis { get; set; } 
 public string? Duration { get; set; }
}


public class JikanPagination 
{ 
    public bool HasNextPage { get; set; }

}




    public class JikanSearchResponse
{
    [JsonPropertyName("data")]
    public List<JikanAnimeData> Data { get; set; } = [];
}



public class JikanImageSet
{
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("small_image_url")]
    public string? SmallImageUrl { get; set; }

    [JsonPropertyName("large_image_url")]
    public string? LargeImageUrl { get; set; }
}

public class JikanImages
{

     [JsonPropertyName("jpg")]
    public JikanImageSet? Jpg { get; set; }

    [JsonPropertyName("webp")]
    public JikanImageSet? Webp { get; set; }
}
    

   

    public class JikanGenre 
    { 
    public int MalId { get; set; } 
    public required string Name { get; set; } 
     
    }

public class JikanDateRange
{
    [JsonPropertyName("from")]
    public DateTime? From { get; set; }

    [JsonPropertyName("to")]
    public DateTime? To { get; set; }
}



   public class JikanAnimeData
{
    [JsonPropertyName("mal_id")]
    public int MalId { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("images")]

    public JikanImages? Images { get; set; }

    [JsonPropertyName("score")]
    public float? Score { get; set; }

    [JsonPropertyName("synopsis")]
    public string? Synopsis { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("episodes")]
    public int? Episodes { get; set; }

     [JsonPropertyName("status")]
    public string? Status { get; set; }
    public JikanDateRange? Aired { get; set; }

    public List<JikanGenre> Genres { get; set; } = [];
}




public class JikanMangaResponse
{
    public JikanMangaData? Data { get; set; }
}


public class JikanMangaData
{
    [JsonPropertyName("mal_id")]
    public int MalId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("images")]
    public JikanImages? Images { get; set; }

    [JsonPropertyName("score")]
    public float? Score { get; set; }

    [JsonPropertyName("synopsis")]
    public string? Synopsis { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("chapters")]
    public int? Chapters { get; set; }

    [JsonPropertyName("volumes")]
    public int? Volumes { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("published")]
    public JikanDateRange? Published { get; set; }
}

public class JikanMangaApiResponse<T>
{
    public List<T>? Data { get; set; }
}



public class JikanAired
{
    [JsonPropertyName("from")]
    public DateTime? From { get; set; }

    [JsonPropertyName("to")]
    public DateTime? To { get; set; }

    [JsonPropertyName("string")]
    public string? String { get; set; }
}

   



