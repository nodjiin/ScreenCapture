using Core.Dtos;
using ScreenCapture.WebApp.Domain;

namespace ScreenCapture.WebApp.Services.Interfaces
{
    public interface IRemoteAgentCommunicationManager
    {
        Task<CaptureOperationReport> StartRecordingAsync(IRemoteAgent agent, RecordingOptions options);
        Task<CaptureOperationReport> StopRecordingAsync(IRemoteAgent agent);
        Task<CaptureOperationReport> TakeSnapshotAsync(IRemoteAgent agent, ScreenshotOptions options);
        Task<RemoteAgentStatus> GetStatusAsync(IRemoteAgent agent);
    }
}
