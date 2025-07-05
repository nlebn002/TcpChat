
using System.Net.Sockets;
using System.Text;
using Bagira.Shared;

Console.Write("Please enter Name: ");
string? userName = Console.ReadLine();

if (string.IsNullOrWhiteSpace(userName))
{
    Console.WriteLine("Name is required.");
    return;
}

try
{
    using TcpClient client = new TcpClient();
    await client.ConnectAsync(Utils.ServerIP, Utils.ServerPort);
    Console.WriteLine(Utils.FormatWithTimestamp($"Connected to chat server at {Utils.ServerIP}:{Utils.ServerPort}"));

    using NetworkStream stream = client.GetStream();
    using var reader = new StreamReader(stream, Encoding.UTF8);
    using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

    // Send user name as handshake
    await writer.WriteLineAsync(userName);

    // Start background task to listen for incoming messages
    _ = Task.Run(async () =>
    {
        try
        {
            while (true)
            {
                string? incoming = await reader.ReadLineAsync();
                if (incoming != null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(incoming);
                    Console.ResetColor();
                    //Console.Write("> ");
                }
            }
        }
        catch
        {
            Console.WriteLine(Utils.FormatWithTimestamp("Connection lost."));
        }
    });

    // Input loop: read user messages and send
    string? input;
    Console.WriteLine("You can type messages or use TO:<Name>-<Message> for specific client.");
    Console.Write("> ");
    while ((input = Console.ReadLine()) != null)
    {
        if (!string.IsNullOrEmpty(input))
            await writer.WriteLineAsync(input);

        int currentLineCursor = Console.CursorTop - 1;
        Console.SetCursorPosition(0, currentLineCursor);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(Utils.FormatWithTimestamp($"You: {input}", true));
        Console.ResetColor();

        //Console.Write("> ");
    }
}
catch (Exception ex)
{
    Console.WriteLine(Utils.FormatWithTimestamp($"Failed to connect or communicate: {ex.Message}", true));
}