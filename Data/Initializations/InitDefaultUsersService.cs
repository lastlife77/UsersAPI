// Copyright (c) Cargorun.

using Data.Options;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace Data.Repositories.Initialization;

public class InitDefaultUsersService
{
    private readonly UsersOptions _usersOptions;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public InitDefaultUsersService(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher,
        IOptions<UsersOptions> usersOptions)
    {
        _usersOptions = usersOptions.Value;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task InitUsersAsync()
    {
        foreach (var user in _usersOptions.Users)
        {
            var isExist = await _userRepository
                .GetAllUsers()
                .Where(x => x.Login == user.Login)
                .AnyAsync();

            if (!isExist) 
            {
                User createUser = new User
                {
                    Login = user.Login,
                    PasswordHash = _passwordHasher.Generate(user.Password),
                    Name = user.Name,
                    Admin = user.Admin,
                };

                await _userRepository.AddAsync(createUser);
            }
        }
    }
}

