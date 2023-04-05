using System.ComponentModel.DataAnnotations;

namespace CaptureAgent.Configurations;
public class MonitorConfiguration
{
    [Range(1, int.MaxValue)]
    public int Width { get; set; }
    [Range(1, int.MaxValue)]
    public int Height { get; set; }
}

