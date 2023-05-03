using ScreenCapture.WebApp.Domain;

namespace ScreenCapture.WebApp.Services.Interfaces;

public interface IMediaExplorer
{
    Task<string[]> FindStoredScreenshots();
    Task<string[]> FindStoredVideos();
    Task<MediaInfo?> GetScreenshotInformation(string name);
    Task<MediaInfo?> GetVideoInformation(string name);
}
