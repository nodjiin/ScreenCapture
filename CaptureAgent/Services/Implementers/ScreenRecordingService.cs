using CaptureAgent.Configurations;
using CaptureAgent.Domain;
using CaptureAgent.Services.Interfaces;
using Core.Domain;
using Core.Dtos;
using Core.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace CaptureAgent.Services.Implementers;
public class ScreenRecordingService : IScreenRecordingService
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ScreenRecordingServiceConfiguration _config;
    private readonly string _fullPath;
    private readonly string _remoteSavePath;
    private readonly IVideoRecorder _recorder;
    private readonly IFileTransferService _transferService;
    private readonly IMetadataFileManager _metadataManager;

    public ScreenRecordingService(IOptions<ScreenRecordingServiceConfiguration> config, IVideoRecorder recorder, IFileTransferService transferService, IMetadataFileManager metadataManager)
    {
        _config = config.Value;
        _fullPath = Path.GetFullPath(_config.LocalSavePath);
        _remoteSavePath = _config.RemoteSavePath;
        _recorder = recorder;
        _transferService = transferService;
        _metadataManager = metadataManager;
    }

    public RecordingStatus Status { get; set; }

    public async Task StartRecordingAsync(RecordingOptions options)
    {
        await _semaphore.WaitAsync();
        if (Status == RecordingStatus.Recording)
        {
            _semaphore.Release();
            return;
        }

        Status = RecordingStatus.Recording;

        try
        {
            await _recorder.StartRecordingAsync(options, _fullPath).ConfigureAwait(false);
        }
        catch (Exception)
        {
            Status = RecordingStatus.Idle;
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<string> StopRecordingAsync()
    {
        await _semaphore.WaitAsync();
        if (Status == RecordingStatus.Idle)
        {
            _semaphore.Release();
            return string.Empty;
        }

        Status = RecordingStatus.Idle;
        MediaCreationReport<VideoMetadata> report;

        try
        {
            report = await _recorder.StopRecordingAsync().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }

        if (string.IsNullOrWhiteSpace(report.FileName))
        {
            throw new InvalidOperationException("Failed to create video file.");
        }

        await _transferService.SendFileAsync(Path.Combine(_fullPath, report.FileName), _remoteSavePath).ConfigureAwait(false);
        string metadataFilePath = Path.Combine(_fullPath, Path.ChangeExtension(report.FileName, "xml"));
        await _metadataManager.CreateMetadataFileAsync(report.Metadata, metadataFilePath).ConfigureAwait(false);
        await _transferService.SendFileAsync(Path.Combine(_fullPath, metadataFilePath), _config.RemoteSavePath).ConfigureAwait(false);

        return report.FileName;
    }
}
