using Microsoft.Extensions.Options;
using ScreenCapture.WebApp.Configurations;
using ScreenCapture.WebApp.Domain;
using ScreenCapture.WebApp.Services.Interface;

namespace ScreenCapture.WebApp.Services.Implementers
{
    public class RemoteAgentsMonitor : IRemoteAgentsMonitor, IDisposable
    {
        private static readonly TimeSpan _statusCheckInterval = TimeSpan.FromSeconds(10); // TODO parameter?
        private PeriodicTimer _timer;
        private bool _isStarted;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public RemoteAgentsMonitor(IOptions<List<RemoteAgentConfiguration>> config, IHttpClientFactory httpClientFactory)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            HttpClient http = httpClientFactory.CreateClient();
            RemoteAgents = new List<IRemoteAgent>();
            foreach (var agentConfig in config.Value)
            {
                RemoteAgents.Add(new RemoteAgent(agentConfig, http));
            }
        }

        public List<IRemoteAgent> RemoteAgents { get; }

        public async Task StartMonitoring()
        {
            if (_isStarted)
            {
                return;
            }

            await CheckAgentsStatus();

            _timer = new PeriodicTimer(_statusCheckInterval);
            _ = Task.Run(DoAsyncPings);
            _isStarted = true;
        }

        private async Task CheckAgentsStatus()
        {
            foreach (var agent in RemoteAgents)
            {
                await agent.UpdateStatusAsync();
            }
        }

        private async Task DoAsyncPings()
        {
            while (await _timer.WaitForNextTickAsync())
            {
                await CheckAgentsStatus();
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
