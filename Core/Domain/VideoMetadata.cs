namespace Core.Domain;

public class VideoMetadata : Metadata
{
    public TimeSpan? Duration { get; set; }
    public string? Codec { get; set; }
    public int Size { get; set; }
    public int BitRate { get; set; }
}
