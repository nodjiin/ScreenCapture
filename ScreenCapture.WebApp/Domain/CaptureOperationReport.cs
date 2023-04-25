using System.Net;

namespace ScreenCapture.WebApp.Domain;
public class CaptureOperationReport
{
    private CaptureOperationReport() { }

    public bool IsSuccessful { get; init; }
    public RemoteAgentStatus AgentStatusAfterOperation { get; init; }
    public string? NewFileName { get; init; }
    public HttpStatusCode StatusCode { get; init; }

    public static CaptureOperationReport SuccessfulReport(RemoteAgentStatus status, string? newFileName = null) => new CaptureOperationReport()
    {
        IsSuccessful = true,
        AgentStatusAfterOperation = RemoteAgentStatus.Online,
        NewFileName = newFileName
    };

    public static CaptureOperationReport ErrorReport(HttpStatusCode statusCode) => new CaptureOperationReport()
    {
        IsSuccessful = false,
        AgentStatusAfterOperation = RemoteAgentStatus.Error,
        StatusCode = statusCode
    };

    public static CaptureOperationReport OfflineReport() => new CaptureOperationReport()
    {
        IsSuccessful = false,
        AgentStatusAfterOperation = RemoteAgentStatus.Offline
    };
}
