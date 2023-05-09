using Core.Domain;
using ScreenCapture.WebApp.Domain;

namespace ScreenCapture.WebApp.Services.Interfaces;

public interface IMediaExplorer
{
    Task<string[]> FindStoredScreenshots();
    Task<string[]> FindStoredVideos();
    Task<MediaInfo<Metadata>?> GetScreenshotInformation(string name);
    Task<MediaInfo<VideoMetadata>?> GetVideoInformation(string name);
}
