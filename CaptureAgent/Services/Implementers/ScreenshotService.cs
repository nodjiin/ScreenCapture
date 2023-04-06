using CaptureAgent.Configurations;
using CaptureAgent.Services.Interfaces;
using Core.Dtos;
using Microsoft.Extensions.Options;

namespace CaptureAgent.Services.Implementers;
public class ScreenshotService : IScreenshotService
{
    private readonly string _fullPath;
    private readonly ScreenshotServiceConfiguration _config;
    private readonly IScreenSnapper _snapper;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public ScreenshotService(IOptions<ScreenshotServiceConfiguration> config, IScreenSnapper screenSnapper)
    {
        _config = config.Value;
        _fullPath = Path.GetFullPath(_config.SavePath);
        _snapper = screenSnapper;
    }

    public async Task<string> TakeScreenshotAsync(ScreenshotOptions options)
    {
        string snapshotName;
        await _semaphore.WaitAsync();

        try
        {
            snapshotName = await _snapper.TakeScreenshotAsync(options, _fullPath).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }

        // TODO send new file to server

        return snapshotName;
    }
}
