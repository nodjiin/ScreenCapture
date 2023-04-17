using CaptureAgent.Configurations;
using CaptureAgent.Services.Interfaces;
using FluentFTP;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace CaptureAgent.Services.Implementers;

// The current implementation uses FluentFTP to handle the server connection. I'm creating the client class here
// instead of injecting it to encapsulate all the references to the FluentFTP nuget package within this single class.
public class FileTransferService : IFileTransferService
{
    private readonly ILogger<FileTransferService> _logger;
    private readonly AsyncFtpClient _client;

    public FileTransferService(IOptions<FtpServerConfiguration> config, ILogger<FileTransferService> logger)
    {
        _logger = logger;

        FtpServerConfiguration _config = config.Value;
        X509Certificate2 certificate = new(_config.CertificatePath);
        FtpConfig ftpConfig = new()
        {
            DataConnectionEncryption = true,
            EncryptionMode = FtpEncryptionMode.Explicit,
            DataConnectionType = FtpDataConnectionType.PASV
        };
        ftpConfig.ClientCertificates.Add(certificate);
        _client = new(_config.Ip, _config.UserName, _config.Password, _config.Port, ftpConfig);
        _client.ValidateCertificate += (c, e) => e.Accept = true;
    }

    public async Task SendFileAsync(string filePath, bool deleteLocalFile = true)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"No file to send has been found at path: '{filePath}'.");
        }

        await _client.Connect().ConfigureAwait(false);
        await _client.UploadFile(filePath, Path.GetFileName(filePath)).ConfigureAwait(false);
        await _client.Disconnect().ConfigureAwait(false);

        if (deleteLocalFile)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to cleanup the file '{filePath}'.");
            }
        }
        else
        {
            _logger.LogInformation($"Keeping local version of file '{filePath}'.");
        }
    }
}
