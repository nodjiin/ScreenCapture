using Core.Dtos;
using ScreenCapture.WebApp.Domain;
using ScreenCapture.WebApp.Services.Interfaces;
using Core.Extension;
using Core.Helpers;

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

    // An exception raised in during any get/post request to the agent likely means that the remote agent is offline,
    // therefore it's going to be simply logged as a warning

    public async Task<RemoteAgentStatus> StartRecordingAsync(IRemoteAgent agent, RecordingOptions options)
    {
        var endpoint = _startRecordingEndpointTemplate.Replace("<ip:port>", IpPort(agent));
        try
        {
            var response = await _client.PostAsync(endpoint, HttpContentGenerator.ConvertToJson(options)).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogUnsuccessfulHttpResponse(response);
                return RemoteAgentStatus.Error;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Exception raised while trying to assert the status of {agent}");
            return RemoteAgentStatus.Offline;
        }

        return RemoteAgentStatus.Recording;
    }

    public async Task<RemoteAgentStatus> StopRecordingAsync(IRemoteAgent agent)
    {
        var endpoint = _stopRecordingEndpointTemplate.Replace("<ip:port>", IpPort(agent));
        try
        {
            var response = await _client.PostAsync(endpoint, null).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogUnsuccessfulHttpResponse(response);
                return RemoteAgentStatus.Error;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Exception raised while trying to assert the status of {agent}");
            return RemoteAgentStatus.Offline;
        }

        return RemoteAgentStatus.Online;
    }

    public async Task<RemoteAgentStatus> TakeSnapshotAsync(IRemoteAgent agent, ScreenshotOptions options)
    {
        var endpoint = _takeSnapshotEndpointTemplate.Replace("<ip:port>", IpPort(agent));

        try
        {
            var response = await _client.PostAsync(endpoint, HttpContentGenerator.ConvertToJson(options)).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogUnsuccessfulHttpResponse(response);
                return RemoteAgentStatus.Error;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Exception raised while trying to assert the status of {agent}");
            return RemoteAgentStatus.Offline;
        }

        return RemoteAgentStatus.Recording;
    }

    public async Task<RemoteAgentStatus> GetStatusAsync(IRemoteAgent agent)
    {
        var endpoint = _getStatusEndpointTemplate.Replace("<ip:port>", IpPort(agent));

        try
        {
            var response = await _client.GetAsync(endpoint).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogUnsuccessfulHttpResponse(response);
                return RemoteAgentStatus.Error;
            }

            var statusDto = await response.Content.ReadFromJsonAsync<CaptureAgentStatus>().ConfigureAwait(false);
            if (statusDto == null)
            {
                _logger.LogError("Endpoint returned a malformed response.");
                return RemoteAgentStatus.Error;
            }

            return statusDto.RecordingStatus == RecordingStatus.Idle ? RemoteAgentStatus.Online : RemoteAgentStatus.Recording;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Exception raised while trying to assert the status of {agent}");
            return RemoteAgentStatus.Offline;
        }
    }

    private string IpPort(IRemoteAgent agent) => $"{agent.Ip}:{agent.Port}";
}
