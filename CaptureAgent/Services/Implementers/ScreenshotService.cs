using CaptureAgent.Configurations;
using CaptureAgent.Domain;
using CaptureAgent.Services.Interfaces;
using Core.Domain;
using Core.Dtos;
using Core.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace CaptureAgent.Services.Implementers;
public class ScreenshotService : IScreenshotService
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly string _fullPath;
    private readonly ScreenshotServiceConfiguration _config;
    private readonly IScreenSnapper _snapper;
    private readonly IFileTransferService _transferService;
    private readonly IMetadataFileManager _metadataManager;

    public ScreenshotService(IOptions<ScreenshotServiceConfiguration> config, IScreenSnapper screenSnapper, IFileTransferService transferService, IMetadataFileManager metadataManager)
    {
        _config = config.Value;
        _fullPath = Path.GetFullPath(_config.LocalSavePath);
        _snapper = screenSnapper;
        _transferService = transferService;
        _metadataManager = metadataManager;
    }

    public async Task<string> TakeScreenshotAsync(ScreenshotOptions options)
    {
        MediaCreationReport<Metadata> report;

        await _semaphore.WaitAsync();

        try
        {
            report = await _snapper.TakeScreenshotAsync(options, _fullPath).ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }

        if (string.IsNullOrWhiteSpace(report.FileName))
        {
            throw new InvalidOperationException("Failed to create the screenshot file.");
        }

        await _transferService.SendFileAsync(Path.Combine(_fullPath, report.FileName), _config.RemoteSavePath).ConfigureAwait(false);
        string metadataFilePath = Path.Combine(_fullPath, Path.ChangeExtension(report.FileName, "xml"));
        await _metadataManager.CreateMetadataFileAsync(report.Metadata, metadataFilePath).ConfigureAwait(false);
        await _transferService.SendFileAsync(Path.Combine(_fullPath, metadataFilePath), _config.RemoteSavePath).ConfigureAwait(false);

        return report.FileName;
    }
}
