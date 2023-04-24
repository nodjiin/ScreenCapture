using Core.Dtos;
using ScreenCapture.WebApp.Domain;
using ScreenCapture.WebApp.Services.Interfaces;

namespace ScreenCapture.WebApp.Services.Implementers;
public class RemoteAgentCommunicationManager : IRemoteAgentCommunicationManager
{
    private static readonly string _startRecordingEndpointTemplate = "https://<ip:port>/Capture/StartRecoding";
    private static readonly string _stopRecordingEndpointTemplate = "https://<ip:port>/Capture/StopRecording";
    private static readonly string _takeSnapshotEndpointTemplate = "https://<ip:port>/Capture/TakeSnapshot";
    private static readonly string _getStatusEndpointTemplate = "https://<ip:port>/Status/GetStatus";
    private readonly HttpClient _client;
    private readonly ILogger<RemoteAgentCommunicationManager> _logger;

    public RemoteAgentCommunicationManager(ILogger<RemoteAgentCommunicationManager> logger, HttpClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<RemoteAgentStatus> StartRecordingAsync(IRemoteAgent agent, RecordingOptions options)
    {
        var endpoint = _startRecordingEndpointTemplate.Replace("<ip:port>", IpPort(agent));

        throw new NotImplementedException();
    }

    public async Task<RemoteAgentStatus> StopRecordingAsync(IRemoteAgent agent)
    {
        var endpoint = _stopRecordingEndpointTemplate.Replace("<ip:port>", IpPort(agent));

        throw new NotImplementedException();
    }

    public async Task<RemoteAgentStatus> TakeSnapshotAsync(IRemoteAgent agent, ScreenshotOptions options)
    {
        var endpoint = _takeSnapshotEndpointTemplate.Replace("<ip:port>", IpPort(agent));

        throw new NotImplementedException();
    }

    public async Task<RemoteAgentStatus> GetStatusAsync(IRemoteAgent agent)
    {
        var endpoint = _getStatusEndpointTemplate.Replace("<ip:port>", IpPort(agent));

        try
        {
            var response = await _client.GetAsync(endpoint).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Unsuccessful call to GetStatusAsync with code: '{response.StatusCode}' and reason: '{response.ReasonPhrase}'");
                return RemoteAgentStatus.Error;
            }

            var statusDto = await response.Content.ReadFromJsonAsync<CaptureAgentStatus>().ConfigureAwait(false);
            if (statusDto == null)
            {
                _logger.LogError("GetStatusAsync returned a malformed response.");
                return RemoteAgentStatus.Error;
            }

            return statusDto.RecordingStatus == RecordingStatus.Idle ? RemoteAgentStatus.Online : RemoteAgentStatus.Recording;
        }
        catch (Exception ex)
        {
            // An exception raised in this stage likely means that the remote agent is offline, so it's going to be threated
            // as a warning
            _logger.LogWarning(ex, $"Exception raised while trying to assert the status of {agent.ToString()}");
            return RemoteAgentStatus.Offline;
        }
    }

    private string IpPort(IRemoteAgent agent) => $"{agent.Ip}:{agent.Port}";
}
