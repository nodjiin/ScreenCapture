using Core.Dtos;

namespace CaptureAgent.Services.Interfaces;
public interface IScreenSnapper
{
    public Task<string> TakeScreenshotAsync(ScreenshotOptions options, string folderPath);
}
