using Application.Dto;
using Application.Exceptions;
using Application.Mappers;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands;

public class UserCommands
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public UserCommands(IPasswordHasher passwordHasher, 
        IUserRepository userRepository,
        IJwtProvider jwtProvider)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task CreateUserAsync(CreateUserDto userDto, string loginByCreated)
    {
        await IsExistUserAsync(userDto.Login);

        userDto.Password = _passwordHasher.Generate(userDto.Password);

        var user = new User
        {
            Login = userDto.Login,
            PasswordHash = userDto.Password,
            Name = userDto.Name,
            Gender = userDto.Gender,
            Birthday = userDto.Birthday,
            Admin = userDto.Admin,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = loginByCreated
        };

        await _userRepository.AddAsync(user);
    }

    public async Task<DetailedUserDto> GetMeAsync(LoginUserDto loginDto)
    {
        await VerifyUserAsync(loginDto.Login, loginDto.Password);

        var user = await _userRepository
            .GetActiveUsers()
            .Where(x => x.Login == loginDto.Login)
            .MapToDetailedUserDto()
            .FirstOrDefaultAsync();

        if (user == null) throw new UnauthorizedException();

        return user;
    }

    public async Task<string> GetTokenAsync(LoginUserDto loginDto)
    {
        await VerifyUserAsync(loginDto.Login, loginDto.Password);

        var isAdmin = await _userRepository
            .GetActiveUsers()
            .Where(x => (x.Login == loginDto.Login) && (x.Admin == true))
            .AnyAsync();

        return _jwtProvider.GenerateToken(loginDto.Login, isAdmin);
    }

    public async Task DeleteUserByLoginAsync(string loginForDelete, string loginByDelete)
    {
        var userForDelete = await GetActiveUserByLoginAsync(loginForDelete);

        userForDelete.RevokedOn = DateTime.UtcNow;
        userForDelete.RevokedBy = loginByDelete;

        await _userRepository.SaveChangesAsync();
    }

    public async Task RestoreUserByLoginAsync(string login)
    {
        var user = await GetRevokedUserByLoginAsync(login);

        user.RevokedOn = null;
        user.RevokedBy = null;

        await _userRepository.SaveChangesAsync();
    }

    public async Task ChangeUserByLoginAsync(string login, ChangeUserDto userDto, string loginByModified)
    {
        var user = await GetActiveUserByLoginAsync(login);

        user.Name = userDto.Name;
        user.Gender = userDto.Gender;
        user.Birthday = userDto.Birthday;
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = loginByModified;

        await _userRepository.SaveChangesAsync();
    }

    public async Task ChangePasswordByLoginAsync(string login, ChangePasswordDto dto, string loginByModified)
    {
        var user = await GetActiveUserByLoginAsync(login);

        await VerifyUserAsync(login, dto.OldPassword);

        user.PasswordHash = _passwordHasher.Generate(dto.NewPassword);
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = loginByModified;

        await _userRepository.SaveChangesAsync();
    }

    public async Task ChangeLoginAsync(string login, string newLogin, string loginByModified)
    {
        var user = await GetActiveUserByLoginAsync(login);

        await IsExistUserAsync(newLogin);

        user.Login = newLogin;
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = loginByModified;

        await _userRepository.SaveChangesAsync();
    }

    private async Task VerifyUserAsync(string login, string password)
    {
        var passwordHash = await _userRepository
               .GetActiveUsers()
               .Where(x => x.Login == login)
               .Select(x => x.PasswordHash)
               .FirstOrDefaultAsync();

        if (passwordHash == null) throw new UnauthorizedException("Неверный логин или пароль");

        var passwordVerifyResult = _passwordHasher.Verify(password, passwordHash);

        if (passwordVerifyResult == false) throw new UnauthorizedException("Неверный логин или пароль");
    }

    private async Task<User> GetActiveUserByLoginAsync(string login)
    {
        var user = await _userRepository
            .GetActiveUsers()
            .Where(x => x.Login == login)
            .FirstOrDefaultAsync();

        if (user == null) throw new NotFoundException("Пользователь не найден");

        return user;
    }

    private async Task<User> GetRevokedUserByLoginAsync(string login)
    {
        var user = await _userRepository
            .GetRevokedUsers()
            .Where(x => x.Login == login)
            .FirstOrDefaultAsync();

        if (user == null) throw new NotFoundException("Пользователь не найден");

        return user;
    }

    private async Task IsExistUserAsync(string login)
    {
        var isExist = await _userRepository.GetAllUsers()
            .Where(x => x.Login == login)
            .AnyAsync();

        if (isExist) throw new ConflictException("Пользователь с таким логином уже существует");
    }
}