using Core.Dtos;
using ScreenCapture.WebApp.Configurations;
using ScreenCapture.WebApp.Services.Interfaces;

namespace ScreenCapture.WebApp.Domain;

public class RemoteAgent : IRemoteAgent
{
    private readonly IRemoteAgentCommunicationManager _communicationManager;
    private RemoteAgentStatus _status = RemoteAgentStatus.Offline;

    public RemoteAgent(RemoteAgentConfiguration config, IRemoteAgentCommunicationManager communicationManager)
    {
        Ip = config.Ip;
        Label = string.IsNullOrWhiteSpace(config.Label) ? config.Ip : config.Label;
        Port = config.Port;
        _communicationManager = communicationManager;
    }

    public event Func<Task>? OnStatusChanged;

    public string Ip { get; init; }
    public string Port { get; init; }
    public string Label { get; init; }
    public RemoteAgentStatus Status
    {
        get => _status;
        private set
        {
            _status = value;
            _ = OnStatusChanged?.Invoke();
        }
    }

    public DateTime? LastOnline { get; private set; }

    // TODO think about the necessity of locking during status update.
    // a direct lock might be avoidable, since the capture agent perform checks on it's status
    // before each operation, but I need to think this through

    public async Task UpdateStatusAsync()
    {
        Status = await _communicationManager.GetStatusAsync(this).ConfigureAwait(false);
    }

    public async Task StartRecordingAsync(RecordingOptions options)
    {
        Status = await _communicationManager.StartRecordingAsync(this, options).ConfigureAwait(false);
    }

    public async Task StopRecordingAsync()
    {
        Status = await _communicationManager.StopRecordingAsync(this).ConfigureAwait(false);
    }
    public async Task TakeScreenshotAsync(ScreenshotOptions options)
    {
        Status = await _communicationManager.TakeSnapshotAsync(this, options).ConfigureAwait(false);
    }

    public override string ToString()
    {
        return $"[Agent - Label:'{Label}' -- Ip:'{Ip}' -- Port:'{Port}']";
    }
}

public enum RemoteAgentStatus
{
    Offline,
    Online,
    Recording,
    Error
}