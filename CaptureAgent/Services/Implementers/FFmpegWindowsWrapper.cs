using CaptureAgent.Configurations;
using CaptureAgent.Services.Interfaces;
using Core.Dtos;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Versioning;
using System.Text;

namespace CaptureAgent.Services.Implementers;

[SupportedOSPlatform("windows")]
public class FFmpegWindowsWrapper : IVideoRecorder
{
    private readonly FFmpegConfiguration _config;
    private readonly Process _ffmpeg;
    private string _filename = string.Empty;

    public FFmpegWindowsWrapper(IOptions<FFmpegConfiguration> config)
    {
        _config = config.Value;
        ProcessStartInfo info = new(_config.ApplicationPath)
        {
            CreateNoWindow = true,
            RedirectStandardInput = true,
            UseShellExecute = false
        };
        _ffmpeg = new Process
        {
            StartInfo = info
        };
    }

    public Task StartRecordingAsync(RecordingOptions options, string folderPath) => Task.Run(() =>
    {
        _ffmpeg.StartInfo.Arguments = GenerateFFmpegArguments(options, folderPath);
        _ffmpeg.Start();
    });

    //TODO argument validation
    private string GenerateFFmpegArguments(RecordingOptions options, string folderPath)
    {
        _filename = $"{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss", CultureInfo.InvariantCulture)}.mkv";

        StringBuilder @string = new StringBuilder("-f gdigrab");

        if (!string.IsNullOrWhiteSpace(options.DrawMouse))
        {
            @string.Append(" -draw_mouse ");
            @string.Append(options.DrawMouse);
        }

        if (!string.IsNullOrWhiteSpace(options.FrameRate))
        {
            @string.Append(" -framerate ");
            @string.Append(options.FrameRate);
        }

        if (!string.IsNullOrWhiteSpace(options.ShowRegion))
        {
            @string.Append(" -show_region ");
            @string.Append(options.ShowRegion);
        }

        if (!string.IsNullOrWhiteSpace(options.VideoSize))
        {
            @string.Append(" -video_size ");
            @string.Append(options.VideoSize);
        }

        if (!string.IsNullOrWhiteSpace(options.OffsetX))
        {
            @string.Append(" -offset_x ");
            @string.Append(options.OffsetX);
        }

        if (!string.IsNullOrWhiteSpace(options.OffsetY))
        {
            @string.Append(" -offset_y ");
            @string.Append(options.OffsetY);
        }

        if (!string.IsNullOrWhiteSpace(options.WindowTitle))
        {
            @string.Append(" -i title=");
            @string.Append(options.WindowTitle);
            @string.Append(' ');
        }
        else
        {
            @string.Append(" -i desktop ");
        }

        @string.Append(Path.Combine(folderPath, _filename));
        return @string.ToString();
    }

    public async Task<string> StopRecordingAsync()
    {
        await _ffmpeg.StandardInput.WriteAsync("q").ConfigureAwait(false);
        await _ffmpeg.WaitForExitAsync().ConfigureAwait(false);
        return _filename;
    }
}

