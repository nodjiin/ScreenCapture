using CaptureAgent.Services.Interfaces;
using Core.Dtos;

namespace CaptureAgent.Services.Implementers;
public class ScreenshotService : IScreenshotService
{
    public Task<string> TakeScreenshot(ScreenshotOptions options)
    {
        throw new NotImplementedException();
    }
}
