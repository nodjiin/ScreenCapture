using CaptureAgent.Services.Interfaces;

namespace CaptureAgent.Services.Implementers;
public class FileTransferService : IFileTransferService
{
    public Task SendFileAsync(string filePath, bool deleteLocalFile = true)
    {
        return Task.CompletedTask;
    }
}
