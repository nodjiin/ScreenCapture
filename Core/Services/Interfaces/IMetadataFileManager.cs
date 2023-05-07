namespace Core.Services.Interfaces;

public interface IMetadataFileManager
{
    public Task CreateMetadataFileAsync<Tmeta>(Tmeta metadata, string path);
    public Task<Tmeta?> ParseMetadataFileAsync<Tmeta>(string path) where Tmeta : class;
}
