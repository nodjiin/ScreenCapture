namespace CaptureAgent.Configurations
{
    public class ScreenRecordingServiceConfiguration
    {
        [ValidPath]
        public string SavePath { get; set; } = string.Empty;
    }
}
