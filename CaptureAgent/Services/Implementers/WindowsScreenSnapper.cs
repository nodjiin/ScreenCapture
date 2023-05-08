using CaptureAgent.Services.Interfaces;
using Core.Dtos;
using System.Drawing.Imaging;
using System.Drawing;
using System.Globalization;
using System.Runtime.Versioning;
using CaptureAgent.Configurations;
using Microsoft.Extensions.Options;
using CaptureAgent.Domain;
using Core.Domain;

namespace CaptureAgent.Services.Implementers;

[SupportedOSPlatform("windows")]
public class WindowsScreenSnapper : IScreenSnapper
{
    private readonly MonitorConfiguration _monitorConfig;
    private readonly string _machineName;

    public WindowsScreenSnapper(IOptions<MonitorConfiguration> monitorConfig, IConfiguration config)
    {
        _monitorConfig = monitorConfig.Value;
        _machineName = config.GetSection(Const.MachineNameKey).Value ?? string.Empty;
    }

    public Task<MediaCreationReport<Metadata>> TakeScreenshotAsync(ScreenshotOptions options, string folderPath) => Task.Run(() =>
    {
        DateTime creationDate = DateTime.Now;

        // TODO multi-monitor, specific region selection, etc.
        (ImageFormat format, string fileExtension) = ValidateFormat(options.ImageFormat);
        string newImageName = $"{creationDate.ToString("yyyy-MM-dd_HH.mm.ss", CultureInfo.InvariantCulture)}.{fileExtension}";
        using Bitmap screenshot = new Bitmap(_monitorConfig.Width, _monitorConfig.Height);
        using Graphics g = Graphics.FromImage(screenshot);
        g.CopyFromScreen(0, 0, 0, 0, screenshot.Size, CopyPixelOperation.SourceCopy);
        screenshot.Save($"{folderPath}{newImageName}", format);

        Metadata metadata = new()
        {
            CaptureMachine = _machineName,
            Type = fileExtension,
            Width = screenshot.Width,
            Eight = screenshot.Height,
            CreationDate = creationDate,
        };

        return new MediaCreationReport<Metadata>()
        {
            FileName = newImageName,
            Metadata = metadata
        };
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
