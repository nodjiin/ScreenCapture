using System.ComponentModel;

namespace Core.Dtos;

// The input device currently used to grab video from the screen is GDI screengrabber.
// For more info take a look at https://ffmpeg.org/ffmpeg-devices.html#gdigrab
public class RecordingOptions
{
    [DefaultValue(null)]
    public string? DrawMouse { get; set; }

    [DefaultValue(null)]
    public string? FrameRate { get; set; }

    [DefaultValue(null)]
    public string? ShowRegion { get; set; }

    [DefaultValue(null)]
    public string? VideoSize { get; set; }

    [DefaultValue(null)]
    public string? OffsetX { get; set; }

    [DefaultValue(null)]
    public string? OffsetY { get; set; }

    [DefaultValue(null)]
    public string? WindowTitle { get; set; }
}
