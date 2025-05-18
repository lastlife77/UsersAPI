using Application.Dto;
using Domain.Entities;

namespace Application.Mappers;

public static class UserMapper
{
    public static IQueryable<DetailedUserDto> MapToDetailedUserDto(this IQueryable<User> entity)
    {
        return entity.Select(entity => new DetailedUserDto
        {
            Login = entity.Login,
            Name = entity.Name,
            Gender = entity.Gender,
            Birthday = entity.Birthday,
            Admin = entity.Admin
        });
    }

    public static IQueryable<UserDto> MapToUserDto(this IQueryable<User> entity)
    {
        return entity.Select(entity => new UserDto
        {
            Name = entity.Name,
            Gender = entity.Gender,
            Birthday = entity.Birthday,
            Active = entity.RevokedOn == null
        });
    }
}