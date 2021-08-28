using LiveChat.Business.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveChat.Business.HostedServices
{
    public class SessionsControlBackground : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<SessionsControlBackground> _logger;
        private readonly ISessionsControl _sessionService;
        private Timer _timer;

        public SessionsControlBackground(ILogger<SessionsControlBackground> logger, IServiceScopeFactory factory)
        {
            _logger = logger;
            _sessionService = factory.CreateScope().ServiceProvider.GetRequiredService<ISessionsControl>();
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count}", count);
            _sessionService.Run();


        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
