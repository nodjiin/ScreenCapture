namespace Core.Dtos;

public class CaptureAgentStatus
{
    public RecordingStatus RecordingStatus { get; set; }
}

public enum RecordingStatus
{
    Idle,
    Recording,
}
