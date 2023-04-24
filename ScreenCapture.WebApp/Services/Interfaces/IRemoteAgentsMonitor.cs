using ScreenCapture.WebApp.Domain;

namespace ScreenCapture.WebApp.Services.Interfaces
{
    public interface IRemoteAgentsMonitor
    {
        public List<IRemoteAgent> RemoteAgents { get; }
        public Task StartMonitoring();
    }
}
