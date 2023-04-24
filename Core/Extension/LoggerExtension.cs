using Microsoft.Extensions.Logging;
using System.Text;

namespace Core.Extension
{
    public static class LoggerExtension
    {
        public static void LogUnsuccessfulHttpResponse<T>(this ILogger<T> logger, HttpResponseMessage response, string? message = default)
        {
            StringBuilder errorString = new StringBuilder();
            errorString.Append("Unsuccessful Http request: ");
            errorString.Append(response.RequestMessage);
            errorString.Append("\n");
            errorString.Append("The response has error code: '");
            errorString.Append(response.StatusCode);
            errorString.Append("' and reason '");
            errorString.Append(response.ReasonPhrase);
            errorString.Append("'.");
            if (!string.IsNullOrWhiteSpace(message))
            {
                errorString.Append("\n");
                errorString.Append("Additional message: ");
                errorString.Append(message);
            }

            logger.LogError(errorString.ToString());
        }
    }
}
