using Application.DataAnnotations;

namespace Application.Dto;

public class ChangePasswordDto
{
    public string OldPassword { get; set; } = null!;

    [LatinLettersAndDigitsOnly(ErrorMessage = "Пароль должен содержать только латинские буквы и цифры.")]
    public string NewPassword { get; set; } = null!;
}
