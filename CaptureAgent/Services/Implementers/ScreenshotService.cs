using CaptureAgent.Configurations;
using CaptureAgent.Services.Interfaces;
using Core.Dtos;
using Microsoft.Extensions.Options;

namespace CaptureAgent.Services.Implementers;
public class ScreenshotService : IScreenshotService
{
    public readonly ScreenshotServiceConfiguration _config;

    public ScreenshotService(IOptions<ScreenshotServiceConfiguration> config)
    {
        _config = config.Value;
    }

    public Task<string> TakeScreenshot(ScreenshotOptions options)
    {
        string test = _config.SavePath;
        throw new NotImplementedException();
    }
}
