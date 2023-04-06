using CaptureAgent.Services.Interfaces;
using Core.Dtos;
using System.Runtime.Versioning;

namespace CaptureAgent.Services.Implementers;

[SupportedOSPlatform("windows")]
public class FFmpegWindowsWrapper : IVideoRecorder
{
    public Task StartRecordingAsync(RecordingOptions options, string folderPath)
    {
        throw new NotImplementedException();
    }

    public Task<string> StopRecordingAsync()
    {
        throw new NotImplementedException();
    }
}

