using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bagira.Server
{
    public class ChatServer : IHostedService
    {
        private readonly IChatTransport _transport;
        private readonly ILogger<ChatServer> _logger;

        public ChatServer(IChatTransport transport, ILogger<ChatServer> logger)
        {
            _transport = transport;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting BagiraChat Server...");
            _transport.Start(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping BagiraChat Server...");
            _transport.Stop();
            return Task.CompletedTask;
        }
    }
}
