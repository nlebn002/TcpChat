using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;
using System.Text;
using Bagira.Shared;

var clients = new ConcurrentDictionary<string, StreamWriter>();
var listener = new TcpListener(IPAddress.Loopback, Utils.ServerPort);
var letterDictionary = new ConcurrentDictionary<char, int>();

Console.WriteLine(Utils.FormatWithTimestamp($"Chat server listening on port {Utils.ServerPort}..."));
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


        Console.WriteLine(Utils.FormatWithTimestamp($"{clientName} connected."));
        await SendMessage($"{clientName} joined the chat", exclude: clientName);

        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (line.StartsWith("TO:", StringComparison.OrdinalIgnoreCase))
                await SendPersonalMessage(clientName, line);
            else
                await SendMessage($"{clientName}: {line}", exclude: clientName);

            CountAndPrintLetterStats(line);
        }
    }
    catch
    {
        Console.WriteLine(Utils.FormatWithTimestamp($"{clientName ?? "Unknown"} disconnected with error."));
    }
    finally
    {
        if (clientName != null && clients.TryRemove(clientName, out _))
        {
            Console.WriteLine(Utils.FormatWithTimestamp($"{clientName} disconnected."));
            await SendMessage($"{clientName} left the chat", exclude: clientName);
        }

        client.Close();
    }
}


async Task SendMessage(string message, string exclude = null)
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
            Console.WriteLine(Utils.FormatWithTimestamp($"Failed to send to {name}"));
        }
    }
}

async Task SendPersonalMessage(string sender, string rawMessage)
{
    // Format: TO:Bob|Hello there!
    var parts = rawMessage[3..].Split('-', 2, StringSplitOptions.TrimEntries);
    if (parts.Length != 2)
    {
        if (clients.TryGetValue(sender, out var senderWriter))
            await senderWriter.WriteLineAsync(Utils.FormatWithTimestamp("Invalid format. Use TO:Name-Message content"));
        return;
    }

    var recipient = parts[0];
    var message = parts[1];

    if (recipient == sender)
        return;

    if (clients.TryGetValue(recipient, out var recipientWriter))
    {
        await recipientWriter.WriteLineAsync($"Private from {sender}: {message}");
    }
    else if (clients.TryGetValue(sender, out var senderWriter))
    {
        await senderWriter.WriteLineAsync($"User '{recipient}' not found.");
    }
}


void CountAndPrintLetterStats(string message)
{
    CountLetters(message);
    PrintLetterStats();
}

void CountLetters(string message)
{
    foreach (var ch in message.ToLower())
    {
        if (!char.IsLetter(ch)) continue;

        if (!letterDictionary.ContainsKey(ch))
            letterDictionary[ch] = 0;
        letterDictionary[ch]++;
    }
}

void PrintLetterStats()
{
    Console.WriteLine(Utils.FormatWithTimestamp("Letter counts:"));
    foreach (var kvp in letterDictionary.OrderBy(k => k.Key))
        Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}