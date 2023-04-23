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
    public RemoteAgentStatus Status { get; }

    public Task UpdateStatusAsync(HttpClient http)
    {
        throw new NotImplementedException();
    }

    public Task StartRecordingAsync(HttpClient http) { throw new NotImplementedException(); }

    public Task StopRecordingAsync(HttpClient http) { throw new NotImplementedException(); }
}

public enum RemoteAgentStatus
{
    Online,
    Offline,
    Recording
}