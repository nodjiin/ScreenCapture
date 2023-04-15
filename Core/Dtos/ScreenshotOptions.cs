using System.ComponentModel;

namespace Core.Dtos;
public class ScreenshotOptions
{
    [DefaultValue("jpeg")]
    public string? ImageFormat { get; set; }
}

