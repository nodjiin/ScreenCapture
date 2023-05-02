using Core.Attributes;
using System.ComponentModel;

namespace Core.Dtos;

// The input device currently used to grab video from the screen is GDI screengrabber.
// For more info take a look at https://ffmpeg.org/ffmpeg-devices.html#gdigrab
[SettingKey("video")]
public class RecordingOptions
{
    [DefaultValue(null)]
    [SettingKey("draw_mouse")]
    public string? DrawMouse { get; set; }

    [DefaultValue(null)]
    [SettingKey("framerate")]
    public string? FrameRate { get; set; }

    [DefaultValue(null)]
    [SettingKey("show_region")]
    public string? ShowRegion { get; set; }

    [DefaultValue(null)]
    [SettingKey("video_size")]
    public string? VideoSize { get; set; }

    [DefaultValue(null)]
    [SettingKey("offset_x")]
    public string? OffsetX { get; set; }

    [DefaultValue(null)]
    [SettingKey("offset_y")]
    public string? OffsetY { get; set; }

    [DefaultValue(null)]
    [SettingKey("window_title")]
    public string? WindowTitle { get; set; }
}
