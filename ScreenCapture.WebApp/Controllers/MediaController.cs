using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScreenCapture.WebApp.Services.Interfaces;

namespace ScreenCapture.WebApp.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class MediaController : Controller
    {
        private readonly IMediaExplorer _explorer;
        private readonly IWebHostEnvironment _environment;
        public MediaController(IMediaExplorer explorer, IWebHostEnvironment environment)
        {
            _explorer = explorer;
            _environment = environment;
        }

        [HttpGet("video")]
        public async Task<IActionResult> VideosList()
        {
            return Ok(await _explorer.FindStoredVideos());
        }

        [HttpGet("video/{name}")]
        public async Task<IActionResult> Video([FromRoute] string name)
        {
            var info = await _explorer.GetVideoInformation(name);
            if (info == null)
            {
                return BadRequest("The requested video has not been found.");
            }

            return File(System.IO.File.OpenRead(info.Path), $"video/{info.Metadata?.Type}", enableRangeProcessing: true); // TODO I need an 'extension to content type' converter
        }

        [HttpGet("screenshot")]
        public async Task<IActionResult> ScreenshotsList()
        {
            return Ok(await _explorer.FindStoredScreenshots());
        }

        [HttpGet("screenshot/{name}")]
        public async Task<IActionResult> Screenshot([FromRoute] string name)
        {
            var info = await _explorer.GetScreenshotInformation(name);
            if (info == null)
            {
                return BadRequest("The requested screenshot has not been found."); // TODO I need an 'extension to content type' converter
            }

            return PhysicalFile(info.Path, $"image/{info.Metadata?.Type}");
        }
    }
}
