namespace Domain.Contracts;

public interface IPasswordHasher
{
    string Generate(string password);

    bool Verify(string password, string passwordHash);
}
