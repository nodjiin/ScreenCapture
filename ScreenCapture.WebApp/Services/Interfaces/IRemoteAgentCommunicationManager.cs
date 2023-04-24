using Core.Dtos;
using ScreenCapture.WebApp.Domain;

namespace ScreenCapture.WebApp.Services.Interfaces
{
    public interface IRemoteAgentCommunicationManager
    {
        Task<RemoteAgentStatus> StartRecordingAsync(IRemoteAgent agent, RecordingOptions options);
        Task<RemoteAgentStatus> StopRecordingAsync(IRemoteAgent agent);
        Task<RemoteAgentStatus> TakeSnapshotAsync(IRemoteAgent agent, ScreenshotOptions options);
        Task<RemoteAgentStatus> GetStatusAsync(IRemoteAgent agent);
    }
}
