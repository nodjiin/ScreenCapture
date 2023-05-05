using System.ComponentModel;

namespace CaptureAgent.Configurations
{
    public class ScreenRecordingServiceConfiguration
    {
        [ValidPath]
        public string LocalSavePath { get; set; } = string.Empty;

        [DefaultValue("videos")]
        public string RemoteSavePath { get; set; } = string.Empty;
    }
}
