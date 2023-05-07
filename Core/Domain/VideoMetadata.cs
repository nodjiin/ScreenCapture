namespace Core.Domain;

public class VideoMetadata : Metadata
{
    public string? Codec { get; set; }
    public TimeSpan? Duration { get; set; }
}
