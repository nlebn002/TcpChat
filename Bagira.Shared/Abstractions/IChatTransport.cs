namespace Bagira.Shared.Abstractions;

interface IChatTransport : IDisposable
{
    Task ConnectAsync();
    Task SendAsync(string message);
    Task<string?> ReceiveAsync();
}
