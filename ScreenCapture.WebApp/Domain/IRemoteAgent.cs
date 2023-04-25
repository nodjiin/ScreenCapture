using Core.Dtos;

namespace ScreenCapture.WebApp.Domain
{
    public interface IRemoteAgent : INotifyStatusChanged
    {
        string Ip { get; init; }
        string Label { get; init; }
        string Port { get; init; }
        DateTime? LastOnline { get; }
        RemoteAgentStatus Status { get; }

        Task<CaptureOperationReport> StartRecordingAsync(RecordingOptions options);
        Task<CaptureOperationReport> StopRecordingAsync();
        Task<CaptureOperationReport> TakeScreenshotAsync(ScreenshotOptions options);
        Task UpdateStatusAsync();
    }
}