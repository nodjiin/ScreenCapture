using Core.Dtos;

namespace CaptureAgent.Services.Interfaces;

public interface IVideoRecorder
{
    public Task StartRecordingAsync(RecordingOptions options, string folderPath);
    public Task<string> StopRecordingAsync();
}

