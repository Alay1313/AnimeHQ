using AutoMapper;
using Domain;

namespace Application;

public class EpisodeService : IEpisodeService
{
    private readonly IEpisodeRepo _repo;
    private readonly IMapper _mapper;
    private readonly IJikanService _jikan;

  public EpisodeService(IEpisodeRepo repo, IMapper mapper, IJikanService jikan)
    {
        _repo = repo;
        _mapper = mapper;
        _jikan = jikan;
    }
    public async Task<EpisodeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var episode = await _repo.GetByIdAsync(id, ct);
        return episode == null ? null : _mapper.Map<EpisodeDto>(episode);
    }

    public async Task<IEnumerable<EpisodeDto>> GetByAnimeIdAsync(int animeId, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var episodes = await _repo.GetByAnimeIdPagedAsync(animeId, page, pageSize, ct);
        return _mapper.Map<IEnumerable<EpisodeDto>>(episodes);
    }

    public async Task<EpisodeDto?> CreateAsync(CreateEpisodeDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Episode>(dto);
        var created = await _repo.CreateAsync(entity, ct);
        return _mapper.Map<EpisodeDto>(created);
    }

    public async Task<EpisodeDto?> UpdateAsync(int id, UpdateEpisodeDto dto, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        var updated = await _repo.UpdateAsync(id, existing, ct);
        return updated == null ? null : _mapper.Map<EpisodeDto>(updated);
    }



    public async Task<IEnumerable<EpisodeDto>> SyncFromJikanAsync(int animeId, CancellationToken ct = default)
    {
        var jikanEpisodes = await _jikan.GetEpisodesAsync(animeId);
        if (jikanEpisodes?.Data == null) throw new Exception("Failed to fetch episodes from Jikan.");

        // Delete existing episodes for this anime before re-syncing
        await _repo.DeleteByAnimeIdAsync(animeId, ct);

        var episodes = _mapper.Map<IEnumerable<Episode>>(jikanEpisodes.Data);
        foreach (var episode in episodes)
        {
            episode.AnimeId = animeId;
            await _repo.CreateAsync(episode, ct);
        }

    return await GetByAnimeIdAsync(animeId, ct: ct);
}

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(id, ct);
    }
}
