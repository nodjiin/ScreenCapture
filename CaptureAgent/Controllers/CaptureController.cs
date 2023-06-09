using CaptureAgent.Services.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CaptureAgent.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[ApiVersion("1.0")]
public class CaptureController : ControllerBase
{
    private readonly ILogger<CaptureController> _logger;
    private readonly IScreenRecordingService _screenRecordingService;
    private readonly IScreenshotService _snapshotService;

    public CaptureController(ILogger<CaptureController> logger, IScreenRecordingService screenRecordingService, IScreenshotService snapshotService)
    {
        _logger = logger;
        _screenRecordingService = screenRecordingService;
        _snapshotService = snapshotService;
    }

    [HttpPost]
    public async Task<IActionResult> StartRecording([FromBody] RecordingOptions options)
    {
        _logger.LogInformation("Received new start recording request.");
        try
        {
            await _screenRecordingService.StartRecordingAsync(options).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // TODO specialize status code once exceptions are defined
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> StopRecording()
    {
        string newFileName;

        _logger.LogInformation("Received new stop recording request.");
        try
        {
            newFileName = await _screenRecordingService.StopRecordingAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // TODO specialize status code once exceptions are defined
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok(newFileName);
    }

    [HttpPost]
    public async Task<IActionResult> TakeSnapshot([FromBody] ScreenshotOptions options)
    {
        string newFileName;

        _logger.LogInformation("Received new snapshot request.");
        try
        {
            newFileName = await _snapshotService.TakeScreenshotAsync(options).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // TODO specialize status code once exceptions are defined
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok(newFileName);
    }
}
