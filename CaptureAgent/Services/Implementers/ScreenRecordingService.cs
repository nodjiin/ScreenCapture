using CaptureAgent.Services.Interfaces;
using Core.Dtos;

namespace CaptureAgent.Services.Implementers;
public class ScreenRecordingService : IScreenRecordingService // TODO CancellationToken
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly IVideoRecorder _recorder;

    public ScreenRecordingService(IVideoRecorder recorder)
    {
        _recorder = recorder;
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
            await _recorder.StartRecordingAsync(options).ConfigureAwait(false);
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
            // TODO send recorded file to server
        }
        finally
        {
            _semaphore.Release();
        }

        return recordedVideo;
    }
}
