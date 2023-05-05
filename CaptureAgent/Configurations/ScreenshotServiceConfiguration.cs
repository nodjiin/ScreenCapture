using System.ComponentModel;

namespace CaptureAgent.Configurations;
public class ScreenshotServiceConfiguration
{
    [ValidPath]
    public string LocalSavePath { get; set; } = string.Empty;

    [DefaultValue("screenshots")]
    public string RemoteSavePath { get; set; } = string.Empty;
}
