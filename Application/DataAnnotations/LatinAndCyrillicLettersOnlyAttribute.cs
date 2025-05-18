namespace Application.DataAnnotations;

using System.ComponentModel.DataAnnotations;

public class LatinAndCyrillicLettersOnlyAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null) return false; 

        var str = value.ToString();

        if (string.IsNullOrEmpty(str)) return false;

        return System.Text.RegularExpressions.Regex.IsMatch(str, @"^[a-zA-Zа-яА-Я]+$");
    }
}