using Core.Dtos;

namespace CaptureAgent.Services.Interfaces;
public interface IScreenshotService
{
    public Task<string> TakeScreenshot(ScreenshotOptions options);
}
