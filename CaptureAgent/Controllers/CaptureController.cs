using CaptureAgent.Services.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CaptureAgent.Controllers;

[ApiController]
[Route("[controller]")]
public class CaptureController : ControllerBase
{
    private readonly ILogger<CaptureController> _logger;    // TODO choose a log library an add logs
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

        try
        {
            newFileName = await _snapshotService.TakeScreenshot(options).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // TODO specialize status code once exceptions are defined
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok(newFileName);
    }
}
