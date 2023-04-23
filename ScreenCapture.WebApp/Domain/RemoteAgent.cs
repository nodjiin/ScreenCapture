using ScreenCapture.WebApp.Configurations;

namespace ScreenCapture.WebApp.Domain;

public class RemoteAgent : IRemoteAgent
{
    private readonly HttpClient _client;

    public RemoteAgent(RemoteAgentConfiguration config, HttpClient client)
    {
        Ip = config.Ip;
        Label = string.IsNullOrWhiteSpace(config.Label) ? config.Ip : config.Label;
        _client = client;
    }

    public string Ip { get; init; }
    public string Label { get; init; }
    public RemoteAgentStatus Status { get; private set; }
    public DateTime? LastOnline { get; private set; }

    public Task UpdateStatusAsync()
    {
        // TODO NEXT
        return Task.CompletedTask;
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