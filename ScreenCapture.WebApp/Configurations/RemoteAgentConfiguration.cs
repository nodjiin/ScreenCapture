using System.ComponentModel.DataAnnotations;

namespace ScreenCapture.WebApp.Configurations
{
    public class RemoteAgentConfiguration
    {
        [Required]
        public string Ip { get; set; } = string.Empty;

        public string? Label { get; set; }
    }
}
