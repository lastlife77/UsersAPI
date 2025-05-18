namespace Tests.Domain;

public class UserTests
{
    [Theory]
    [InlineData(2, 6, 4)]
    [InlineData(0, -5, 4)]
    [InlineData(9, 0, 4)]
    public void CreateUser_Failed(double a, double b, double c)
    {
    }
}
