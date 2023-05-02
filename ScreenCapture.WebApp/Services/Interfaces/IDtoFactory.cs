namespace ScreenCapture.WebApp.Services.Interfaces;

public interface IDtoFactory
{
    public Task<TDto> CreateSettingDtoAsync<TDto>() where TDto : new();
}
