namespace CaptureAgent.Services.Interfaces;
public interface IFileTransferService
{
    public Task SendFileAsync(string filePath, bool deleteLocalFile = true);
}
