namespace Domain.Contracts;

public interface IJwtProvider
{
    string GenerateToken(string login, bool admin);
}
