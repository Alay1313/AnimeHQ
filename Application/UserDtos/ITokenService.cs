using Domain;

namespace Application;

public interface ITokenService
{
    string GenerateToken(User user);
}