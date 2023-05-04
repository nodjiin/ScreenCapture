using ScreenCapture.WebApp.Domain;
using ScreenCapture.WebApp.Services.Interfaces;

namespace ScreenCapture.WebApp.Services.Implementers;

public class LocalDiskMediaExplorer : IMediaExplorer
{
    private const string videoSubfolder = "videos";
    private const string screenshotSubfolder = "screenshots";
    private readonly string _videoSubfolderFullPath;
    private readonly string _screenshotSubfolderFullPath;
    private readonly ILogger<LocalDiskMediaExplorer> _logger;

    public LocalDiskMediaExplorer(ILogger<LocalDiskMediaExplorer> logger, IConfiguration configuration)
    {
        _logger = logger;
        string localDiskMediaFolder = configuration.GetValue<string>("MediaPath") ?? throw new InvalidOperationException("The path to the media storage has not been configured");
        _videoSubfolderFullPath = Path.Combine(localDiskMediaFolder, videoSubfolder);
        _screenshotSubfolderFullPath = Path.Combine(localDiskMediaFolder, screenshotSubfolder);
    }

    public Task<string[]> FindStoredVideos()
    {
        return Task.Run(() => GetDirectoryContentNames(_videoSubfolderFullPath));
    }

    public Task<string[]> FindStoredScreenshots()
    {
        return Task.Run(() => GetDirectoryContentNames(_screenshotSubfolderFullPath));
    }

    private string[] GetDirectoryContentNames(string directory)
    {
        if (!Directory.Exists(directory))
        {
            _logger.LogError($"Media sub folder '{directory}' has not been found at the given path.");
            return Array.Empty<string>();
        }

        string[] content;

        try
        {
            content = Directory.EnumerateFiles(directory).Select(v => Path.GetFileName(v)).ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception raised while trying to access media directory '{directory}'");
            content = Array.Empty<string>();
        }

        return content;
    }

    public Task<MediaInfo?> GetVideoInformation(string name)
    {
        return Task.Run(() => GetMediaInformation(_videoSubfolderFullPath, name));
    }

    public Task<MediaInfo?> GetScreenshotInformation(string name)
    {
        return Task.Run(() => GetMediaInformation(_screenshotSubfolderFullPath, name));
    }

    private MediaInfo? GetMediaInformation(string folderPath, string name)
    {
        FileInfo fileInfo;

        try
        {
            var elementPath = Path.Combine(folderPath, name);
            fileInfo = new FileInfo(elementPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception raised while trying to access media element '{name}'");
            return null;
        }

        if (!fileInfo.Exists)
        {
            _logger.LogError($"Requested media element '{name}' has not been found.");
            return null;
        }

        return new MediaInfo()
        {
            Name = fileInfo.Name,
            Path = fileInfo.FullName,
            Extension = fileInfo.Extension
        };
    }
}
