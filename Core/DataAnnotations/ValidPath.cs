using System.ComponentModel.DataAnnotations;

public class ValidPathAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var path = value as string;
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        try
        {
            Path.GetFullPath(path);
            return true;
        }
        catch
        {
            return false;
        }
    }
}