using Application.DataAnnotations;

namespace Application.Dto;

public class ChangeUserDto
{
    [LatinAndCyrillicLettersOnly(ErrorMessage = "Имя должно содержать только латинские или русские буквы.")]
    public string Name { get; set; } = null!;

    public int Gender { get; set; }

    public DateTime? Birthday { get; set; }
}
