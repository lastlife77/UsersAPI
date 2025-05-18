namespace Application.Dto;

public class UserDto
{
    public string Name { get; set; } = null!;

    public int Gender { get; set; }

    public DateTime? Birthday { get; set; }

    public bool Active { get; set; }
}
