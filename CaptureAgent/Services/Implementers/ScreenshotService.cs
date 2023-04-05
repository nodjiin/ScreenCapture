using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.Versioning;
using CaptureAgent.Configurations;
using CaptureAgent.Services.Interfaces;
using Core.Dtos;
using Microsoft.Extensions.Options;

namespace CaptureAgent.Services.Implementers;
public class ScreenshotService : IScreenshotService
{
    public readonly string _fullPath;
    public readonly ScreenshotServiceConfiguration _config;
    public readonly MonitorConfiguration _monitorConfig;

    public ScreenshotService(IOptions<ScreenshotServiceConfiguration> config, IOptions<MonitorConfiguration> monitorConfig)
    {
        _config = config.Value;
        _fullPath = Path.GetFullPath(_config.SavePath);
        _monitorConfig = monitorConfig.Value;
    }

    public Task<string> TakeScreenshot(ScreenshotOptions options) => Task.Run(() =>
    {
        string newImageName;

        if (OperatingSystem.IsWindows())
        {
            // TODO multi-monitor, specific region selection, etc.
            (ImageFormat format, string fileExtension) = ValidateFormat(options.ImageFormat);
            newImageName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss", CultureInfo.InvariantCulture)}.{fileExtension}";
            using Bitmap screenshot = new Bitmap(_monitorConfig.Width, _monitorConfig.Height);
            using Graphics g = Graphics.FromImage(screenshot);
            g.CopyFromScreen(0, 0, 0, 0, screenshot.Size, CopyPixelOperation.SourceCopy);
            screenshot.Save($"{_fullPath}{newImageName}", format);
        }
        else
        {
            // TODO multi-platform implementation
            throw new NotImplementedException();
        }

        return newImageName;
    });

    [SupportedOSPlatform("windows")]
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
