using Bagira.Shared.Abstractions;
using System.Net.Sockets;
using System.Text;

namespace Bagira.Shared
{
    internal class TcpChatTransport(string ip, int port) : IChatTransport, IDisposable
    {
        private readonly string _ip = ip;
        private readonly int _port = port;
        private TcpClient? _client;
        private StreamReader? _reader;
        private StreamWriter? _writer;

        public async Task ConnectAsync()
        {
            _client = new();
            await _client.ConnectAsync(_ip, _port);
            _reader = new(_client.GetStream(), Encoding.UTF8);
            _writer = new(_client.GetStream(), Encoding.UTF8) { AutoFlush = true };
        }

        public async Task<string?> ReceiveAsync()
        {
            if (_reader == null) throw new InvalidOperationException("Transport not connected.");
            try
            {
                return await _reader.ReadLineAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
                return null;
            }
        }

        public async Task SendAsync(string message)
        {
            if (_writer == null) throw new InvalidOperationException("Transport not connected.");
            try
            {
                await _writer.WriteAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing message: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
