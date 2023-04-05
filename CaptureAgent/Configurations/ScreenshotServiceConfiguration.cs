namespace CaptureAgent.Configurations;
public class ScreenshotServiceConfiguration
{
    [ValidPath]
    public string SavePath { get; set; } = string.Empty;
}
