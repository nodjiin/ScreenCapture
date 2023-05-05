namespace CaptureAgent.Services.Interfaces;
public interface IFileTransferService
{
    public Task SendFileAsync(string filePath, string remotePath, bool deleteLocalFile = true);
}
