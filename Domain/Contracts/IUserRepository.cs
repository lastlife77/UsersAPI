using Domain.Entities;

namespace Domain.Contracts;

public interface IUserRepository
{
    Task AddAsync(User user);

    IQueryable<User> GetAllUsers();

    IQueryable<User> GetActiveUsers();

    IQueryable<User> GetRevokedUsers();

    Task SaveChangesAsync();
}
