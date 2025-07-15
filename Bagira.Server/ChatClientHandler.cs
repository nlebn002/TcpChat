using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Bagira.Server
{

    public class ChatClientHandler
    {
        private readonly TcpClient _client;
        private readonly IMessageRouter _router;
        private readonly List<ChatClientHandler> _clients;
        private readonly ILogger _logger;
        private NetworkStream _stream;
        public string NickName { get; private set; } = "Unknown";

        public ChatClientHandler(TcpClient client, IMessageRouter router, List<ChatClientHandler> clients, ILogger logger)
        {
            _client = client;
            _router = router;
            _clients = clients;
            _logger = logger;
            _stream = _client.GetStream();
        }

        public async Task HandleAsync()
        {
            byte[] buffer = new byte[4096];
            try
            {
                await SendAsync("Please enter Name:");
                int length = await _stream.ReadAsync(buffer);
                NickName = Encoding.UTF8.GetString(buffer, 0, length).Trim();

                await SendAsync($"Welcome {NickName}!");
                _logger.LogInformation($"{NickName} connected.");

                while (true)
                {
                    length = await _stream.ReadAsync(buffer);
                    if (length == 0) break;
                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    await _router.RouteMessageAsync(NickName, message, this, _clients);
                }
            }
            catch
            {
                // ignore
            }
            finally
            {
                Disconnect();
            }
        }

        public async Task SendAsync(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message + Environment.NewLine);
            await _stream.WriteAsync(bytes);
        }

        public void Disconnect()
        {
            _logger.LogInformation($"{NickName} disconnected.");
            _client.Close();
            lock (_clients)
            {
                _clients.Remove(this);
            }
        }
    }
}
