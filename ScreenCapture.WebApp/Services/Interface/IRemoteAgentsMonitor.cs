using ScreenCapture.WebApp.Domain;

namespace ScreenCapture.WebApp.Services.Interface
{
    public interface IRemoteAgentsMonitor
    {
        public List<IRemoteAgent> RemoteAgents { get; }
        public Task StartMonitoring();
    }
}
