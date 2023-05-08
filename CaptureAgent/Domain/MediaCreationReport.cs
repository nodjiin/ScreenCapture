namespace CaptureAgent.Domain;

public class MediaCreationReport<Tmetadata>
{
    public string? FileName { get; init; }
    public Tmetadata? Metadata { get; init; }
}
