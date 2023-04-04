using Core.Dtos;

namespace CaptureAgent.Services.Interfaces;

public interface IScreenRecordingService
{
    public Task StartRecordingAsync(RecordingOptions options);
    public Task<string> StopRecordingAsync();
}

