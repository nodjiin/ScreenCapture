namespace ScreenCapture.WebApp.Domain;
public interface INotifyStatusChanged
{
    public event Func<Task>? OnStatusChanged;
}
