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
        var newStatus = await _communicationManager.GetStatusAsync(this).ConfigureAwait(false);
        if (newStatus != RemoteAgentStatus.Offline)
        {
            // TODO Local time is being used both to name captured media file and to keep track of the
            // agent on line present. I need to check how this is going to impact Blazor server on a multiple
            // timezone scenario. This code is running on the server which means the DateTime displayed in the 
            // browser is automatically converted?
            LastOnline = DateTime.Now;
        }

        Status = newStatus;
    }

    public async Task<CaptureOperationReport> StartRecordingAsync(RecordingOptions options)
    {
        var report = await _communicationManager.StartRecordingAsync(this, options).ConfigureAwait(false);
        Status = report.AgentStatusAfterOperation;
        return report;
    }

    public async Task<CaptureOperationReport> StopRecordingAsync()
    {
        var report = await _communicationManager.StopRecordingAsync(this).ConfigureAwait(false);
        Status = report.AgentStatusAfterOperation;
        return report;
    }
    public async Task<CaptureOperationReport> TakeScreenshotAsync(ScreenshotOptions options)
    {
        var report = await _communicationManager.TakeSnapshotAsync(this, options).ConfigureAwait(false);
        return report;
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