using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScreenCapture.WebApp.Domain;
using ScreenCapture.WebApp.Services.Interfaces;

namespace ScreenCapture.WebApp.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class MediaController : Controller
    {
        private readonly IMediaExplorer _explorer;

        public MediaController(IMediaExplorer explorer)
        {
            _explorer = explorer;
        }

        [HttpGet("video")]
        public async Task<IActionResult> VideosList()
        {
            return Ok(await _explorer.FindStoredVideos());
        }

        [HttpGet("video/{name}")]
        public async Task<IActionResult> Video([FromRoute] string name)
        {
            MediaInfo? info = await _explorer.GetVideoInformation(name);
            if (info == null)
            {
                return BadRequest("The requested video has not been found.");
            }

            return PhysicalFile(info.Path, $"video/{info.Extension}"); // TODO I need an 'extension to content type' converter
        }

        [HttpGet("screenshot")]
        public async Task<IActionResult> ScreenshotsList()
        {
            return Ok(await _explorer.FindStoredScreenshots());
        }

        [HttpGet("screenshot/{name}")]
        public async Task<IActionResult> Screenshot([FromRoute] string name)
        {
            MediaInfo? info = await _explorer.GetScreenshotInformation(name);
            if (info == null)
            {
                return BadRequest("The requested screenshot has not been found."); // TODO I need an 'extension to content type' converter
            }

            return PhysicalFile(info.Path, $"image/{info.Extension}");
        }
    }
}
