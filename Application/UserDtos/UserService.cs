using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Identity;
namespace Application;

public class UserService : IUserService
{
    private readonly IUserRepo _repo;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;

    public UserService(IUserRepo repo, IMapper mapper, IPasswordHasher<User> passwordHasher, ITokenService tokenService)
    {
        _repo = repo;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
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

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // 1. Check if user already exists
        var existing = await _repo.GetByUsernameAsync(dto.Username);
        if (existing != null) throw new Exception("Username already exists.");

        // 2. Map DTO to Entity
        var user = _mapper.Map<User>(dto);
        user.Id = Guid.NewGuid().ToString();
        user.CreatedAt = DateTime.UtcNow; // Assuming your User entity has a CreatedAt property

        // 3. Hash the password securely
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        // 4. Save to database
        var created = await _repo.CreateAsync(user);

        // 5. Return Auth Response (You will generate the JWT Token here or in the Controller)
        return new AuthResponseDto
        {
            Id = created.Id,
            Username = created.UserName,
            Email = created.Email,
            Token = _tokenService.GenerateToken(created) 
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // 1. Find user by username or email (You may need to add GetByEmailAsync to your repo)
        var user = await _repo.GetByUsernameAsync(dto.UsernameOrEmail);
        
        // Fallback: if not found by username, try email (optional, requires repo update)
        if (user == null) 
        {
            user = await _repo.GetByEmailAsync(dto.UsernameOrEmail);
        }

        if (user == null) throw new Exception("Invalid credentials.");

        // 2. Verify the password hash
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new Exception("Invalid credentials.");
        }

        // 3. Return Auth Response
        return new AuthResponseDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Token = _tokenService.GenerateToken(user)
        };
    }

    public async Task<UserDto?> UpdateAsync(string id, UpdateUserDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return null;

        _mapper.Map(dto, existing);
        var updated = await _repo.UpdateAsync(id, existing);
        return updated == null ? null : _mapper.Map<UserDto>(updated);
    }
}
