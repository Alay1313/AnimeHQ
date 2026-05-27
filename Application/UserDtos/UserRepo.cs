using System;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
namespace Application;

public class UserRepo(AppDbContext context) : IUserRepo
{
    private readonly AppDbContext _context = context;

    public async Task<User?> GetByIdAsync(string id)
    {
       return await _context.Users.Include(u => u.Reviews).Include(u => u.Favorites).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        
    }
        

    public async Task<User> CreateAsync(User user)
    {
        var e = await _context.Users.AddAsync(user); 
        await _context.SaveChangesAsync(); 
        return e.Entity; 
    }

    public async Task<User?> UpdateAsync(string id, User updatedUser)
    {
        var existing = await GetByIdAsync(id); 
        if (existing == null) return null;
        existing.UserName = updatedUser.UserName; 
        existing.Email = updatedUser.Email; 
        await _context.SaveChangesAsync();
        return existing;
        
    }

   



}
