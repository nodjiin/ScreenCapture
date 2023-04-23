namespace ScreenCapture.WebApp.Objects;

public class RemoteAgent
{
    public RemoteAgent(string ip, string label)
    {
        Ip = ip;
        Label = label;
    }

    public string Ip { get; init; }
    public string Label { get; init; }
    public RemoteAgentStatus Status { get; private set; }

    public DateTime? LastOnline { get; private set; }

    public Task UpdateStatusAsync()
    {
        throw new NotImplementedException();
    }

    public Task StartRecordingAsync()
    {
        Status = RemoteAgentStatus.Recording;
        return Task.CompletedTask;
    }

    public Task StopRecordingAsync()
    {
        Status = RemoteAgentStatus.Online;
        return Task.CompletedTask;
    }
    public Task TakeScreenshotAsync()
    {
        Status = RemoteAgentStatus.Offline;
        return Task.CompletedTask;
    }
}

public enum RemoteAgentStatus
{
    Offline,
    Online,
    Recording
}