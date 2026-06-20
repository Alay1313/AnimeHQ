using System;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using Application;

namespace Persistence;

public class UserRepo : IUserRepo
{
    private readonly AppDbContext _context;

    
    public UserRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
       return await _context.Users
            .Include(u => u.Reviews)
            .Include(u => u.Favorites)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        // Include PasswordHash implicitly by selecting the whole entity
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user); 
        await _context.SaveChangesAsync(); 
        return user; 
    }

    public async Task<User?> UpdateAsync(string id, User updatedUser)
    {
        var existing = await _context.Users.FindAsync(id);
        if (existing == null) return null;

        existing.UserName = updatedUser.UserName; 
        existing.Email = updatedUser.Email; 
        
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<User?> GetByEmailAsync(string email)
{
    return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
}
}
