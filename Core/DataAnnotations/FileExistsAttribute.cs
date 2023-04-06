using System.ComponentModel.DataAnnotations;

public class FileExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var filePath = value as string;
        if (!File.Exists(filePath))
        {
            return new ValidationResult($"File does not exist: {filePath}");
        }
        return ValidationResult.Success;
    }
}