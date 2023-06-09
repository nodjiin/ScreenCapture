﻿using Core.Domain;
using Core.Services.Interfaces;
using ScreenCapture.WebApp.Domain;
using ScreenCapture.WebApp.Services.Interfaces;
using System.Collections.Concurrent;

namespace ScreenCapture.WebApp.Services.Implementers;

public class LocalDiskMediaExplorer : IMediaExplorer
{
    private const string videoSubfolder = "videos";
    private const string screenshotSubfolder = "screenshots";
    private readonly string _videoSubfolderFullPath;
    private readonly string _screenshotSubfolderFullPath;
    private readonly ILogger<LocalDiskMediaExplorer> _logger;
    private readonly IMetadataFileManager _metadataManager;

    // This cache is created following the principle that, due to the monitoring scope of the application, saved videos and images are always created with a unique name and never deleted.
    // TODO add a configurable functionality to purge/recreate the cache.  
    private static readonly ConcurrentDictionary<string, MediaInfo<Metadata>> _screenshotCache = new();
    private static readonly ConcurrentDictionary<string, MediaInfo<VideoMetadata>> _videoCache = new();

    public LocalDiskMediaExplorer(ILogger<LocalDiskMediaExplorer> logger, IConfiguration configuration, IMetadataFileManager metadataManager)
    {
        _logger = logger;
        string localDiskMediaFolder = configuration.GetValue<string>("MediaPath") ?? throw new InvalidOperationException("The path to the media storage has not been configured");
        _videoSubfolderFullPath = Path.Combine(localDiskMediaFolder, videoSubfolder);
        _screenshotSubfolderFullPath = Path.Combine(localDiskMediaFolder, screenshotSubfolder);
        _metadataManager = metadataManager;
    }

    public Task<string[]> FindStoredVideos()
    {
        return Task.Run(() => GetDirectoryContentNames(_videoSubfolderFullPath));
    }

    public Task<string[]> FindStoredScreenshots()
    {
        return Task.Run(() => GetDirectoryContentNames(_screenshotSubfolderFullPath));
    }

    private string[] GetDirectoryContentNames(string directory)
    {
        if (!Directory.Exists(directory))
        {
            _logger.LogError($"Media sub folder '{directory}' has not been found at the given path.");
            return Array.Empty<string>();
        }

        string[] content;

        try
        {
            // filtering out all the xml metadata files
            content = Directory.EnumerateFiles(directory).Where(f => Path.GetExtension(f).ToLowerInvariant() != ".xml").Select(v => Path.GetFileName(v)).ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception raised while trying to access media directory '{directory}'");
            content = Array.Empty<string>();
        }

        return content;
    }

    public async Task<MediaInfo<VideoMetadata>?> GetVideoInformation(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        MediaInfo<VideoMetadata>? info;
        if (!_videoCache.TryGetValue(name, out info))
        {
            info = await GetMediaInformation<VideoMetadata>(_videoSubfolderFullPath, name);
            if (info != null)
            {
                _videoCache.TryAdd(name, info);
            }
        }

        return info;
    }

    public async Task<MediaInfo<Metadata>?> GetScreenshotInformation(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        MediaInfo<Metadata>? info;
        if (!_screenshotCache.TryGetValue(name, out info))
        {
            info = await GetMediaInformation<Metadata>(_screenshotSubfolderFullPath, name);
            if (info != null)
            {
                _screenshotCache.TryAdd(name, info);
            }
        }

        return info;
    }

    private async Task<MediaInfo<Tmetadata>?> GetMediaInformation<Tmetadata>(string folderPath, string name)
        where Tmetadata : class
    {
        FileInfo fileInfo;

        try
        {
            var elementPath = Path.Combine(folderPath, name);
            fileInfo = new FileInfo(elementPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception raised while trying to access media element '{name}'");
            return null;
        }

        if (!fileInfo.Exists)
        {
            _logger.LogError($"Requested media element '{name}' has not been found.");
            return null;
        }

        MediaInfo<Tmetadata> info = new()
        {
            Name = fileInfo.Name,
            Path = fileInfo.FullName
        };

        string metadataFileName = Path.ChangeExtension(fileInfo.FullName, "xml");
        if (File.Exists(metadataFileName))
        {
            info.Metadata = await _metadataManager.ParseMetadataFileAsync<Tmetadata>(metadataFileName);
        }
        else
        {
            _logger.LogWarning($"Could not locate metadata file '{metadataFileName}'");
        }

        return info;
    }
}
