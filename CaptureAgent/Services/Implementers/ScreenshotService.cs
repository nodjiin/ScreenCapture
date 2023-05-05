using CaptureAgent.Configurations;
using CaptureAgent.Services.Interfaces;
using Core.Dtos;
using Microsoft.Extensions.Options;

namespace CaptureAgent.Services.Implementers;
public class ScreenshotService : IScreenshotService
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly string _fullPath;
    private readonly ScreenshotServiceConfiguration _config;
    private readonly IScreenSnapper _snapper;
    private readonly IFileTransferService _transferService;

    public ScreenshotService(IOptions<ScreenshotServiceConfiguration> config, IScreenSnapper screenSnapper, IFileTransferService transferService)
    {
        _config = config.Value;
        _fullPath = Path.GetFullPath(_config.LocalSavePath);
        _snapper = screenSnapper;
        _transferService = transferService;
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

        await _transferService.SendFileAsync(Path.Combine(_fullPath, snapshotName), _config.RemoteSavePath).ConfigureAwait(false);
        return snapshotName;
    }
}
