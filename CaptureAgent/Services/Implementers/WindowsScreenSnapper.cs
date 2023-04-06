using CaptureAgent.Services.Interfaces;
using Core.Dtos;
using System.Drawing.Imaging;
using System.Drawing;
using System.Globalization;
using System.Runtime.Versioning;
using CaptureAgent.Configurations;
using Microsoft.Extensions.Options;

namespace CaptureAgent.Services.Implementers;

[SupportedOSPlatform("windows")]
public class WindowsScreenSnapper : IScreenSnapper
{
    private readonly MonitorConfiguration _monitorConfig;

    public WindowsScreenSnapper(IOptions<MonitorConfiguration> monitorConfig)
    {
        _monitorConfig = monitorConfig.Value;
    }

    public Task<string> TakeScreenshotAsync(ScreenshotOptions options, string folderPath) => Task.Run(() =>
    {
        string newImageName;

        // TODO multi-monitor, specific region selection, etc.
        (ImageFormat format, string fileExtension) = ValidateFormat(options.ImageFormat);
        newImageName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss", CultureInfo.InvariantCulture)}.{fileExtension}";
        using Bitmap screenshot = new Bitmap(_monitorConfig.Width, _monitorConfig.Height);
        using Graphics g = Graphics.FromImage(screenshot);
        g.CopyFromScreen(0, 0, 0, 0, screenshot.Size, CopyPixelOperation.SourceCopy);
        screenshot.Save($"{folderPath}{newImageName}", format);

        return newImageName;
    });

    private (ImageFormat format, string fileExtension) ValidateFormat(string? format)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            throw new ArgumentException($"The given image format '{format}' is invalid");
        }

        // TODO expand
        switch (format.ToLowerInvariant().Trim())
        {
            case "jpeg":
            case "jpg":
                return (ImageFormat.Jpeg, "jpg");
            case "png":
                return (ImageFormat.Png, "png");
            case "bmp":
                return (ImageFormat.Bmp, "bmp");
            default:
                throw new ArgumentException($"The given image format '{format}' is invalid");
        }
    }
}
