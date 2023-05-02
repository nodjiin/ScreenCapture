using Core.Attributes;
using System.ComponentModel;

namespace Core.Dtos;

[SettingKey("screenshot")]
public class ScreenshotOptions
{
    [DefaultValue("jpeg")]
    [SettingKey("image_format")]
    public string? ImageFormat { get; set; }
}

