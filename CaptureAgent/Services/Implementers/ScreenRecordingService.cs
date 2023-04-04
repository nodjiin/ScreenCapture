using CaptureAgent.Services.Interfaces;
using Core.Dtos;

namespace CaptureAgent.Services.Implementers;
public class ScreenRecordingService : IScreenRecordingService
{
    public Task StartRecordingAsync(RecordingOptions options)
    {
        throw new NotImplementedException();
    }

    public Task<string> StopRecordingAsync()
    {
        throw new NotImplementedException();
    }
}
