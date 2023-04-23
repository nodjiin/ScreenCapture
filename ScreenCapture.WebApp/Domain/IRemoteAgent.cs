namespace ScreenCapture.WebApp.Domain
{
    public interface IRemoteAgent
    {
        string Ip { get; init; }
        string Label { get; init; }
        DateTime? LastOnline { get; }
        RemoteAgentStatus Status { get; }

        Task StartRecordingAsync();
        Task StopRecordingAsync();
        Task TakeScreenshotAsync();
        Task UpdateStatusAsync();
    }
}