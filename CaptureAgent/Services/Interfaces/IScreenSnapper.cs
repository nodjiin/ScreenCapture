using CaptureAgent.Domain;
using Core.Domain;
using Core.Dtos;

namespace CaptureAgent.Services.Interfaces;
public interface IScreenSnapper
{
    public Task<MediaCreationReport<Metadata>> TakeScreenshotAsync(ScreenshotOptions options, string folderPath);
}
