using Core.Dtos;

namespace ScreenCapture.WebApp.Services.Interfaces;

public interface IDtoFactory
{
    public Task<RecordingOptions> CreateRecordingOptionsAsync();
    public Task<ScreenshotOptions> CreateScreenshotOptionsAsync();
}
