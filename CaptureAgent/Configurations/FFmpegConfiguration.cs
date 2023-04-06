namespace CaptureAgent.Configurations;
public class FFmpegConfiguration
{
    [FileExists]
    public string ApplicationPath { get; set; } = string.Empty;
}
