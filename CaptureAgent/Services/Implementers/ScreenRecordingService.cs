using CaptureAgent.Configurations;
using CaptureAgent.Services.Interfaces;
using Core.Dtos;
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

    public ScreenRecordingService(IOptions<ScreenRecordingServiceConfiguration> config, IVideoRecorder recorder, IFileTransferService transferService)
    {
        _config = config.Value;
        _fullPath = Path.GetFullPath(_config.LocalSavePath);
        _remoteSavePath = _config.RemoteSavePath;
        _recorder = recorder;
        _transferService = transferService;
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
        string recordedVideo;

        try
        {
            recordedVideo = await _recorder.StopRecordingAsync().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }

        await _transferService.SendFileAsync(Path.Combine(_fullPath, recordedVideo), _remoteSavePath).ConfigureAwait(false);
        return recordedVideo;
    }
}
