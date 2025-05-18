using Application.Dto;
using Application.Exceptions;
using Application.Mappers;
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries;

public class UserQueries
{
    private readonly IUserRepository _userRepository;

    public UserQueries(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<DetailedUserDto>> GetAllActiveUsersAsync()
    {
        return await _userRepository
            .GetActiveUsers()
            .OrderBy(x => x.CreatedOn)
            .MapToDetailedUserDto()
            .ToListAsync();
    }

    public async Task<UserDto> GetUserByLoginAsync(string login)
    {
        var user = await _userRepository
                .GetAllUsers()
                .Where(x => x.Login == login)
                .MapToUserDto()
                .FirstOrDefaultAsync();
        if (user == null) throw new NotFoundException("Пользователь не найден");

        return user;
    }

    public async Task<List<DetailedUserDto>> GetUsersOlderThanAsync(int age)
    {
        return await _userRepository
            .GetActiveUsers()
            .Where(x => (x.Birthday != null) &&
                        (x.Birthday.Value.AddYears(age) < DateTime.UtcNow))
            .MapToDetailedUserDto()
            .ToListAsync();
    }
}
