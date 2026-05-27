using System;
using Domain;
using AutoMapper;
namespace Application;

public class EpisodeService
{
    private readonly IEpisodeRepo _repo;
    private readonly IJikanService _jikan;
    private readonly IMapper _mapper;

    public EpisodeService(IEpisodeRepo repo, IJikanService jikan, IMapper mapper)
    {
        _repo = repo;
        _jikan = jikan;
        _mapper = mapper;
    }

     public async Task<EpisodeDto?> GetByIdAsync(int id)
    {
        var episode = await _repo.GetByIdAsync(id);
        if (episode == null)
        {
            return null;
        }
        else
        {
            return (EpisodeDto?)_mapper.Map<EpisodeDto>(episode);
        }
    }

       public async Task<IEnumerable<EpisodeDto>> GetByAnimeIdAsync(int animeId, int page = 1, int pageSize = 20)
    {
        var episodes = await _repo.GetByAnimeIdAsync(animeId, page, pageSize);
        return _mapper.Map<IEnumerable<EpisodeDto>>(episodes);
    }

      public async Task<EpisodeDto?> CreateAsync(CreateEpisodeDto dto)
    {
        var entity = _mapper.Map<Episode>(dto);
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<EpisodeDto>(created);
    }

      public async Task<EpisodeDto?> UpdateAsync(int id, UpdateEpisodeDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;

        var mapped = _mapper.Map(dto, existing);
        var updated = await _repo.UpdateAsync(id, mapped);
        return updated == null ? null : _mapper.Map<EpisodeDto>(updated);
    }


    public async Task<bool> DeleteAsync(int id)
    {
        return await _repo.DeleteAsync(id);
    }



     

}
