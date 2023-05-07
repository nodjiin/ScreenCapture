using Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;

namespace Core.Services.Implementers;

public class XmlMetadataFileManager : IMetadataFileManager
{
    private readonly ILogger<XmlMetadataFileManager> _logger;

    public XmlMetadataFileManager(ILogger<XmlMetadataFileManager> logger)
    {
        _logger = logger;
    }

    public Task CreateMetadataFileAsync<Tmeta>(Tmeta metadata, string path) => Task.Run(() =>
    {
        if (metadata == null)
        {
            _logger.LogError($"Impossible to write invalid metadata on path '{path}'.");
            return;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            _logger.LogError($"Impossible to write metadata on invalid path '{path}'.");
            return;
        }

        XmlSerializer serializer = Helper<Tmeta>.Serializer;

        try
        {
            using FileStream stream = new(path, FileMode.CreateNew);
            serializer.Serialize(stream, metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception raised while trying to write metadata file on path: '{path}'.");
        }
    });

    public Task<Tmeta?> ParseMetadataFileAsync<Tmeta>(string path) where Tmeta : class
    {
        return Task.Run(() =>
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                _logger.LogError($"Invalid metadata file path '{path}'.");
                return null;
            }

            if (!File.Exists(path))
            {
                _logger.LogError($"Impossible to parse not existing file '{path}'.");
                return null;
            }

            XmlSerializer serializer = Helper<Tmeta>.Serializer;

            try
            {
                using FileStream stream = new(path, FileMode.Open, FileAccess.Read);
                var obj = serializer.Deserialize(stream);
                return obj != null ? (Tmeta)(obj) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception raised while trying to parse metadata file '{path}'.");
                return null;
            }
        });
    }

    private static class Helper<T>
    {
        public static readonly XmlSerializer Serializer = new XmlSerializer(typeof(T));
    }
}
