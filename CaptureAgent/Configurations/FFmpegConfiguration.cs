namespace CaptureAgent.Configurations;
public class FFmpegConfiguration
{
    [FileExists]
    public string FFmpegPath { get; set; } = string.Empty;
    [FileExists]
    public string FFprobePath { get; set; } = string.Empty;
}
