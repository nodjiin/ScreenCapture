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
    private readonly IVideoRecorder _recorder;
    private readonly IFileTransferService _transferService;

    public ScreenRecordingService(IOptions<ScreenRecordingServiceConfiguration> config, IVideoRecorder recorder, IFileTransferService transferService)
    {
        _config = config.Value;
        _fullPath = Path.GetFullPath(_config.SavePath);
        _recorder = recorder;
        _transferService = transferService;
    }

    public bool IsRecording { get; set; }

    public async Task StartRecordingAsync(RecordingOptions options)
    {
        await _semaphore.WaitAsync();
        if (IsRecording)
        {
            _semaphore.Release();
            return;
        }

        IsRecording = true;

        try
        {
            await _recorder.StartRecordingAsync(options, _fullPath).ConfigureAwait(false);
        }
        catch (Exception)
        {
            IsRecording = false;
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
        if (!IsRecording)
        {
            _semaphore.Release();
            return string.Empty;
        }

        IsRecording = false;
        string recordedVideo;

        try
        {
            recordedVideo = await _recorder.StopRecordingAsync().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }

        await _transferService.SendFileAsync(Path.Combine(_fullPath, recordedVideo)).ConfigureAwait(false);
        return recordedVideo;
    }
}
