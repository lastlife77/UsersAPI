using Domain.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<User> _users;

    public UserRepository(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
        _users = appDbContext.Set<User>();
    }

    public async Task AddAsync(User user)
    {
        await _users.AddAsync(user);

        await _dbContext.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public IQueryable<User> GetAllUsers()
    {
        return _users;
    }

    public IQueryable<User> GetActiveUsers()
    {
        return _users.Where(x => x.RevokedOn == null);
    }

    public IQueryable<User> GetRevokedUsers()
    {
        return _users.Where(x => x.RevokedOn != null);
    }
}