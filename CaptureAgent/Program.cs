using CaptureAgent.Configurations;
using CaptureAgent.Services.Implementers;
using CaptureAgent.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// TODO add api versioning
builder.Services.AddSingleton<IScreenRecordingService, ScreenRecordingService>();
builder.Services.AddSingleton<IScreenshotService, ScreenshotService>();
builder.Services.AddSingleton<IFileTransferService, FileTransferService>();

if (OperatingSystem.IsWindows())
{
    builder.Services.AddSingleton<IVideoRecorder, FFmpegWindowsWrapper>();
    builder.Services.AddOptions<FFmpegConfiguration>().Bind(builder.Configuration.GetSection(nameof(FFmpegConfiguration))).ValidateDataAnnotations().ValidateOnStart();
    builder.Services.AddSingleton<IScreenSnapper, WindowsScreenSnapper>();
}
else
{
    // TODO multi-platform implementation
    throw new NotImplementedException("The only OS currently supported is windows.");
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<ScreenRecordingServiceConfiguration>().Bind(builder.Configuration.GetSection(nameof(ScreenRecordingServiceConfiguration))).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<ScreenshotServiceConfiguration>().Bind(builder.Configuration.GetSection(nameof(ScreenshotServiceConfiguration))).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<MonitorConfiguration>().Bind(builder.Configuration.GetSection(nameof(MonitorConfiguration))).ValidateDataAnnotations().ValidateOnStart();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
