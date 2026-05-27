using System;
using AutoMapper;
using Domain;
namespace Application.UserDtos;

public class UserService
{
    private readonly IUserRepo _repo;
    private readonly IMapper _mapper;

    public UserService(IUserRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }


     public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await _repo.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }



    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        var user = await _repo.GetByUsernameAsync(username);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }



    public async Task<UserDto?> CreateAsync(UserDto dto)
    {
        
        var entity = _mapper.Map<User>(dto);
        
        
        if (entity.CreatedAt == default) entity.CreatedAt = DateTime.UtcNow;

        var created = await _repo.CreateAsync(entity);
        return _mapper.Map<UserDto>(created);
    }


      public async Task<UserDto?> UpdateAsync(string id, UserDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;

        
        _mapper.Map(dto, existing);
        
        var updated = await _repo.UpdateAsync(id, existing);
        return updated == null ? null : _mapper.Map<UserDto>(updated);
    }

}
