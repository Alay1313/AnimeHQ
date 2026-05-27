using System;
using Domain;
namespace Application;

public interface IUserRepo
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User> CreateAsync(User user);
    Task<User?> UpdateAsync(string id, User updatedUser);

}
