namespace Application;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(string id);
    Task<UserDto?> GetByUsernameAsync(string username);
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<UserDto?> UpdateAsync(string id, UpdateUserDto dto); // Create a simple UpdateUserDto if needed
}


