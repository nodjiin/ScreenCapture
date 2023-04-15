﻿using CaptureAgent.Configurations;
using CaptureAgent.Services.Interfaces;
using FluentFTP;
using Microsoft.Extensions.Options;

namespace CaptureAgent.Services.Implementers;
public class FileTransferService : IFileTransferService
{
    private readonly FtpServerConfiguration _config;

    public FileTransferService(IOptions<FtpServerConfiguration> config)
    {
        _config = config.Value;
    }

    public async Task SendFileAsync(string filePath, bool deleteLocalFile = true)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"No file to send has been at path: '{filePath}'");
        }

        // The current implementation uses FluentFTP to handle the server connection. I'm creating the client class here
        // instead of injecting it to encapsulate all the references to the FluentFTP nuget package within this single class.
        // TODO FTPS & certificate
        FtpConfig ftpConfig = new()
        {
            DataConnectionEncryption = false,
            EncryptionMode = FtpEncryptionMode.None,

        };

        var client = new AsyncFtpClient(_config.Ip, _config.UserName, _config.Password, _config.Port, ftpConfig);
        await client.Connect();
        await client.UploadFile(filePath, Path.GetFileName(filePath));

        // end FluentFTP

        if (deleteLocalFile)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                // TODO log
            }
        }
        else
        {
            // TODO log
        }

    }
}
