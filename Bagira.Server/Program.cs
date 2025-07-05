using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;
using System.Text;

const int Port = 5000;
var clients = new ConcurrentDictionary<string, StreamWriter>();
var listener = new TcpListener(IPAddress.Loopback, Port);

Console.WriteLine($"Chat server listening on port {Port}...");
listener.Start();

while (true)
{
    var tcpClient = await listener.AcceptTcpClientAsync();
    _ = HandleClientAsync(tcpClient);
}


async Task HandleClientAsync(TcpClient client)
{
    string clientName = null;

    try
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        // First line should be the client name
        clientName = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(clientName))
        {
            await writer.WriteLineAsync("Invalid name.");
            return;
        }

        if (!clients.TryAdd(clientName, writer))
        {
            await writer.WriteLineAsync("Name already in use.");
            return;
        }


        Console.WriteLine($"{clientName} connected.");
        await BroadcastAsync($"{clientName} joined the chat", exclude: clientName);

        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (line.StartsWith("TO:", StringComparison.OrdinalIgnoreCase))
                await RouteMessageAsync(clientName, line);
            else
                await BroadcastAsync($"[{clientName}]: {line}", exclude: clientName);
        }
    }
    catch
    {
        Console.WriteLine($"{clientName ?? "Unknown"} disconnected with error.");
    }
    finally
    {
        if (clientName != null && clients.TryRemove(clientName, out _))
        {
            Console.WriteLine($"{clientName} disconnected.");
            await BroadcastAsync($"{clientName} left the chat", exclude: clientName);
        }

        client.Close();
    }
}


async Task BroadcastAsync(string message, string exclude = null)
{
    foreach (var (name, writer) in clients)
    {
        if (name == exclude) continue;
        try
        {
            await writer.WriteLineAsync(message);
        }
        catch
        {
            Console.WriteLine($"Failed to send to {name}");
        }
    }
}

async Task RouteMessageAsync(string sender, string rawMessage)
{
    // Format: TO:Bob|Hello there!
    var parts = rawMessage[3..].Split('-', 2, StringSplitOptions.TrimEntries);
    if (parts.Length != 2)
    {
        if (clients.TryGetValue(sender, out var senderWriter))
            await senderWriter.WriteLineAsync("Invalid format. Use TO:Name-Message content");
        return;
    }

    var recipient = parts[0];
    var message = parts[1];

    if (clients.TryGetValue(recipient, out var recipientWriter))
    {
        await recipientWriter.WriteLineAsync($"[Private from {sender}]: {message}");
    }
    else if (clients.TryGetValue(sender, out var senderWriter))
    {
        await senderWriter.WriteLineAsync($"User '{recipient}' not found.");
    }
}