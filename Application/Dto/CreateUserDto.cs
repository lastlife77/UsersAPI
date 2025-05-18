using Application.DataAnnotations;

namespace Application.Dto;

public class CreateUserDto
{
    [LatinLettersAndDigitsOnly(ErrorMessage = "Логин должен содержать только латинские буквы и цифры.")]
    public string Login { get; set; } = null!;

    [LatinLettersAndDigitsOnly(ErrorMessage = "Пароль должен содержать только латинские буквы и цифры.")]
    public string Password { get; set; } = null!;

    [LatinAndCyrillicLettersOnly(ErrorMessage = "Имя должно содержать только латинские или русские буквы.")]
    public string Name { get; set; } = null!;

    public int Gender { get; set; }

    public DateTime? Birthday { get; set; }

    public bool Admin { get; set; }
}
