using CaptureAgent.Configurations;
using CaptureAgent.Domain;
using CaptureAgent.Services.Interfaces;
using Core.Domain;
using Core.Dtos;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Versioning;
using System.Text;
using System.Xml;

namespace CaptureAgent.Services.Implementers;

[SupportedOSPlatform("windows")]
public class FFmpegWindowsWrapper : IVideoRecorder
{
    private const string _ffprobeArgs = @"-v error -hide_banner -sexagesimal -of default=noprint_wrappers=0 -print_format xml -show_streams  -select_streams v:0 -show_format";

    private readonly ILogger<FFmpegWindowsWrapper> _logger;
    private readonly Process _ffmpeg;
    private readonly Process _ffprobe;
    private readonly string _machineName;
    private string _filename = string.Empty;
    private RecordingOptions? _recordingOptions;
    private string _filePath = string.Empty;
    private DateTime _startRecordingDateTime;

    public FFmpegWindowsWrapper(IOptions<FFmpegConfiguration> options, IConfiguration config, ILogger<FFmpegWindowsWrapper> logger)
    {
        FFmpegConfiguration ffmpegConfig = options.Value;

        // TODO redirect standard Error, produce logs
        _ffmpeg = new Process
        {
            StartInfo = new(ffmpegConfig.FFmpegPath)
            {
                CreateNoWindow = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            }
        };

        _ffprobe = new Process
        {
            StartInfo = new(ffmpegConfig.FFprobePath)
            {
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        _machineName = config.GetSection(Const.MachineNameKey).Value ?? string.Empty;
        _logger = logger;
    }

    public Task StartRecordingAsync(RecordingOptions options, string folderPath) => Task.Run(() =>
    {
        _startRecordingDateTime = DateTime.Now;
        _filename = $"{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss", CultureInfo.InvariantCulture)}.{options.VideoFormat}";
        _recordingOptions = options;
        _filePath = Path.Combine(folderPath, _filename);
        _ffmpeg.StartInfo.Arguments = GenerateFFmpegArguments();
        _ffmpeg.Start();
    });

    //TODO argument validation
    private string GenerateFFmpegArguments()
    {
        if (_recordingOptions == null)
        {
            return string.Empty;
        }

        StringBuilder @string = new StringBuilder("-f gdigrab");

        if (!string.IsNullOrWhiteSpace(_recordingOptions.DrawMouse))
        {
            bool value = Convert.ToBoolean(_recordingOptions.DrawMouse);
            @string.Append(" -draw_mouse ");
            @string.Append(value ? "1" : "0");
        }

        if (!string.IsNullOrWhiteSpace(_recordingOptions.FrameRate))
        {
            @string.Append(" -framerate ");
            @string.Append(_recordingOptions.FrameRate);
        }

        if (!string.IsNullOrWhiteSpace(_recordingOptions.ShowRegion))
        {
            bool value = Convert.ToBoolean(_recordingOptions.ShowRegion);
            @string.Append(" -show_region ");
            @string.Append(value ? "1" : "0");
        }

        if (!string.IsNullOrWhiteSpace(_recordingOptions.VideoSize))
        {
            @string.Append(" -video_size ");
            @string.Append(_recordingOptions.VideoSize);
        }

        if (!string.IsNullOrWhiteSpace(_recordingOptions.OffsetX))
        {
            @string.Append(" -offset_x ");
            @string.Append(_recordingOptions.OffsetX);
        }

        if (!string.IsNullOrWhiteSpace(_recordingOptions.OffsetY))
        {
            @string.Append(" -offset_y ");
            @string.Append(_recordingOptions.OffsetY);
        }

        if (!string.IsNullOrWhiteSpace(_recordingOptions.WindowTitle))
        {
            @string.Append(" -i title=");
            @string.Append(_recordingOptions.WindowTitle);
            @string.Append(' ');
        }
        else
        {
            @string.Append(" -i desktop ");
        }

        @string.Append(_filePath);
        return @string.ToString();
    }

    public async Task<MediaCreationReport<VideoMetadata>> StopRecordingAsync()
    {
        await _ffmpeg.StandardInput.WriteAsync("q").ConfigureAwait(false);
        if (!_ffmpeg.HasExited)
        {
            await _ffmpeg.WaitForExitAsync().ConfigureAwait(false);
        }

        return new MediaCreationReport<VideoMetadata>()
        {
            FileName = _filename,
            Metadata = await GenerateVideoMetadata(),
        };
    }

    private async Task<VideoMetadata> GenerateVideoMetadata()
    {
        _ffprobe.StartInfo.Arguments = $"{_ffprobeArgs} {_filePath}";

        _ffprobe.Start();
        if (!_ffprobe.HasExited)
        {
            await _ffprobe.WaitForExitAsync();
        }
        TryToLogProcessError(_ffprobe);

        VideoMetadata metadata = new()
        {
            CaptureMachine = _machineName,
            CreationDateTime = _startRecordingDateTime,
            Type = _recordingOptions?.VideoFormat,
        };

        using XmlReader reader = XmlReader.Create(_ffprobe.StandardOutput, new() { Async = true });
        while (await reader.ReadAsync())
        {
            // ffprobe arguments are set up to use only the video 0 stream,
            // so only 1 stream element is expected
            if (reader.Name == "stream" && reader.IsStartElement())
            {
                metadata.Codec = reader.GetAttribute("codec_name");
                metadata.Width = Convert.ToInt32(reader.GetAttribute("width"));
                metadata.Height = Convert.ToInt32(reader.GetAttribute("height"));
                string? duration = reader.GetAttribute("duration");
                if (!string.IsNullOrWhiteSpace(duration))
                {
                    metadata.Duration = TimeSpan.Parse(duration);
                }
                continue;
            }

            if (reader.Name == "format" && reader.IsStartElement())
            {
                metadata.Size = Convert.ToInt32(reader.GetAttribute("size"));
                break;
            }
        }
        reader.Close();
        return metadata;
    }

    private void TryToLogProcessError(Process process)
    {
        string error = process.StandardError.ReadToEnd();
        if (!string.IsNullOrWhiteSpace(error))
        {
            _logger.LogError($"Process '{process.ProcessName}' logged error message:\n{error}");
        }
    }
}

