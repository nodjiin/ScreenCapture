namespace ScreenCapture.WebApp.Domain;

// TODO unify with MediaCreationReport
public class MediaInfo<Tmetadata>
{
    public string Name { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public Tmetadata? Metadata { get; set; }
}
