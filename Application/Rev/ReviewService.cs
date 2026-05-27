using System;
using AutoMapper;
using Persistence;
using Domain;

namespace Application.Rev;

public class ReviewService
{
    private readonly IReviewRepo _repo;
    private readonly IMapper _mapper;

      public ReviewService(IReviewRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }


    public async Task<ReviewDto?> GetByIdAsync(int id)
    {
        var review = await _repo.GetByIdAsync(id);
        return review == null ? null : _mapper.Map<ReviewDto>(review);
    }


    public async Task<IEnumerable<ReviewDto>> GetByAnimeIdAsync(int animeId)
    {
        var reviews = await _repo.GetByAnimeIdAsync(animeId);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }


    public async Task<IEnumerable<ReviewDto>> GetByUserIdAsync(string userId)
    {
        var reviews = await _repo.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }


    public async Task<ReviewDto?> CreateAsync(CreateReviewDto dto)
    {
        var entity = _mapper.Map<Review>(dto);
        entity.CreatedAt = DateTime.UtcNow;
        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<ReviewDto>(created);
    }


    public async Task<ReviewDto?> UpdateAsync(int id, ReviewDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;

        existing.Content = dto.Content;
        existing.Rating = dto.Rating;
        existing.UserId = dto.UserId ?? string.Empty;
        existing.AnimeId = dto.AnimeId;

        var updated = await _repo.UpdateAsync(id, existing);
        return updated == null ? null : _mapper.Map<ReviewDto>(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repo.DeleteAsync(id);
        
    }

}
