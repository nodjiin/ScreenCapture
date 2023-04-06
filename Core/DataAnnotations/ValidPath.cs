using System.ComponentModel.DataAnnotations;

public class ValidPathAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var path = value as string;
        if (string.IsNullOrWhiteSpace(path))
        {
            return new ValidationResult("The path string is null or whitespace.");
        }

        try
        {
            Path.GetFullPath(path);
            return ValidationResult.Success;
        }
        catch
        {
            return new ValidationResult("The path is invalid.");
        }
    }
}