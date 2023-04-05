﻿using Core.Dtos;

namespace CaptureAgent.Services.Interfaces;

public interface IScreenRecordingService
{
    public bool IsRecording { get; set; }
    public Task StartRecordingAsync(RecordingOptions options);
    public Task<string> StopRecordingAsync();
}

