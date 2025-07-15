using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bagira.Server
{
    public class TcpChatTransport : IChatTransport
    {
        private TcpListener? _listener;
        private readonly IMessageRouter _router;
        private readonly ILogger<TcpChatTransport> _logger;
        private readonly List<ChatClientHandler> _clients = new();

        public TcpChatTransport(IMessageRouter router, ILogger<TcpChatTransport> logger)
        {
            _router = router;
            _logger = logger;
        }

        public void Start(CancellationToken cancellationToken)
        {
            _listener = new TcpListener(IPAddress.Any, 5000);
            _listener.Start();
            _logger.LogInformation("Server listening on port 5000...");

            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync(cancellationToken);
                    var handler = new ChatClientHandler(client, _router, _clients, _logger);
                    lock (_clients) { _clients.Add(handler); }
                    _ = handler.HandleAsync();
                }
            }, cancellationToken);
        }

        public void Stop()
        {
            _listener?.Stop();
            lock (_clients)
            {
                foreach (var client in _clients)
                    client.Disconnect();
            }
        }
    }
}
