namespace Core.Dtos;

// The input device currently used to grab video from the screen is GDI screengrabber.
// FOr more info take a look at https://ffmpeg.org/ffmpeg-devices.html#gdigrab
public class RecordingOptions
{
    public string? DrawMouse { get; set; }
    public string? FrameRate { get; set; }
    public string? ShowRegion { get; set; }
    public string? VideoSize { get; set; }
    public string? OffsetX { get; set; }
    public string? OffsetY { get; set; }
    public string? WindowTitle { get; set; }
}
