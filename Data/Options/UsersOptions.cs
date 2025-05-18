namespace Data.Options;


public class UsersOptions
{
    public List<UserOptions> Users { get; set; } = [];
}

public class UserOptions
{
    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool Admin { get; set; }
}