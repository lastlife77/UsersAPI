using Application.Commands;
using Application.Dto;
using Application.Exceptions;
using Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UsersAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[IgnoreAntiforgeryToken]
public class UserController : ControllerBase
{
    private readonly UserCommands _userCommands;
    private readonly UserQueries _userQueries;

    public UserController(UserCommands userCommands, UserQueries userQueries)
    {
        _userCommands = userCommands;
        _userQueries = userQueries;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("CreateUser")]
    public async Task<IResult> CreateUser([FromBody] CreateUserDto user)
    {
        try
        {
            var currentUserLogin = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserLogin == null) return Results.Unauthorized();

            await _userCommands.CreateUserAsync(user, currentUserLogin);

            return Results.Ok();
        }
        catch(ConflictException ex)
        {
            return Results.Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }

    [HttpPost("GetToken")]
    public async Task<IResult> Login([FromBody] LoginUserDto login)
    {
        try
        {
            var token = await _userCommands.GetTokenAsync(login);

            return Results.Ok(token);
        }
        catch (UnauthorizedException)
        {
            return Results.Unauthorized();
        }
        catch (Exception ex) 
            {
            return Results.InternalServerError(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("GetMe")]
    public async Task<IResult> GetMe(LoginUserDto login)
    {
        try
        {
            var currentUserLogin = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserLogin != login.Login)
            {
                return Results.Unauthorized();
            }

            var user = await _userCommands.GetMeAsync(login);

            return Results.Ok(user);
        }

        catch (NotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }

    [Authorize(Roles ="Admin")]
    [HttpGet("GetAllUsers")]
    public async Task<IResult> GetAllUsers()
    {
        try
        {
            var users = await _userQueries.GetAllActiveUsersAsync();

            return Results.Ok(users);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetUser/{login}")]
    public async Task<IResult> GetUser(string login)
    {
        try
        {
            var user = await _userQueries.GetUserByLoginAsync(login);

            return Results.Ok(user);
        }

        catch (NotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetUsersOlderThan/{age}")]
    public async Task<IResult> GetUsersOlderThan(int age)
    {
        try
        {
            var users = await _userQueries.GetUsersOlderThanAsync(age);

            return Results.Ok(users);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("DeleteUser/{login}")]
    public async Task<IResult> DeleteUser(string login)
    {
        try
        {
            var currentUserLogin = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserLogin == null) return Results.Unauthorized();

            await _userCommands.DeleteUserByLoginAsync(login, currentUserLogin);

            return Results.Ok();
        }
        catch (NotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("RestoreUser/{login}")]
    public async Task<IResult> RestoreUser(string login)
    {
        try
        {
            await _userCommands.RestoreUserByLoginAsync(login);

            return Results.Ok();
        }
        catch (NotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("ChangeUser/{login}")]
    public async Task<IResult> ChangeUser(string login, [FromBody] ChangeUserDto dto)
    {
        try
        {
            var currentUserLogin = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (currentUserLogin == null) return Results.Unauthorized();

            if (currentUserLogin != login && currentUserRole != "Admin")
            {
                return Results.Unauthorized();
            }

            await _userCommands.ChangeUserByLoginAsync(login, dto, currentUserLogin);

            return Results.Ok();
        }
        catch (NotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("ChangePassword/{login}")]
    public async Task<IResult> ChangePassword(string login, [FromBody] ChangePasswordDto dto)
    {
        try
        {
            var currentUserLogin = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserLogin == null) return Results.Unauthorized();

            if (currentUserLogin != login && currentUserRole != "Admin")
            {
                return Results.Unauthorized();
            }

            await _userCommands.ChangePasswordByLoginAsync(login, dto, currentUserLogin);

            return Results.Ok();
        }
        catch (UnauthorizedException)
        {
            return Results.Unauthorized();
        }
        catch (NotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("ChangeLogin/{login}")]
    public async Task<IResult> ChangeLogin(string login, [FromBody] string newLogin)
    {
        try
        {
            var currentUserLogin = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserLogin == null) return Results.Unauthorized();

            if (currentUserLogin != login && currentUserRole != "Admin")
            {
                return Results.Unauthorized();
            }

            await _userCommands.ChangeLoginAsync(login, newLogin, currentUserLogin);

            return Results.Ok();
        }
        catch (ConflictException ex)
        {
            return Results.Conflict(ex.Message);
        }
        catch (NotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);
        }
    }
}
