using CaptureAgent.Services.Interfaces;
using Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CaptureAgent.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[ApiVersion("1.0")]
public class StatusController : ControllerBase
{
    private readonly IScreenRecordingService _recordingService;

    public StatusController(IScreenRecordingService recordingService)
    {
        _recordingService = recordingService;
    }

    [HttpGet]
    public IActionResult GetStatus()
    {
        return Ok(new CaptureAgentStatus()
        {
            RecordingStatus = _recordingService.Status
        });
    }
}
