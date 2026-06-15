using AutoMapper;
using Domain;
using Persistence;

namespace Application;

public class ReviewService : IReviewService
{
    private readonly IReviewRepo _repo;
    private readonly IMapper _mapper;

    public ReviewService(IReviewRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<ReviewDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var review = await _repo.GetByIdAsync(id, ct);
        return review == null ? null : _mapper.Map<ReviewDto>(review);
    }

    public async Task<IEnumerable<ReviewDto>> GetByAnimeIdAsync(int animeId, CancellationToken ct = default)
    {
        var reviews = await _repo.GetByAnimeIdAsync(animeId, ct);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<IEnumerable<ReviewDto>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var reviews = await _repo.GetByUserIdAsync(userId, ct);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }

    public async Task<ReviewDto?> CreateAsync(CreateReviewDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Review>(dto);
        entity.CreatedAt = DateTime.UtcNow;
        
        var created = await _repo.CreateAsync(entity, ct);
        return _mapper.Map<ReviewDto>(created);
    }

    public async Task<ReviewDto?> UpdateAsync(int id, UpdateReviewDto dto, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing == null) return null;

        // Use AutoMapper to update only the allowed fields (Content, Rating)
        _mapper.Map(dto, existing);
        
        var updated = await _repo.UpdateAsync(id, existing, ct);
        return updated == null ? null : _mapper.Map<ReviewDto>(updated);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await _repo.DeleteAsync(id, ct);
    }
}
