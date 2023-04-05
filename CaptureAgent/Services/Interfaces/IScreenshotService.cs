using Core.Dtos;

namespace CaptureAgent.Services.Interfaces;
public interface IScreenshotService
{
    public Task<string> TakeScreenshotAsync(ScreenshotOptions options);
}
