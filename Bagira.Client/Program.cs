
using System.Net.Sockets;
using System.Text;

Console.Write("Please enter Name: ");
string? userName = Console.ReadLine();

if (string.IsNullOrWhiteSpace(userName))
{
    Console.WriteLine("Name is required.");
    return;
}

const string serverIp = "127.0.0.1";
const int serverPort = 5000;

try
{
    using TcpClient client = new TcpClient();
    await client.ConnectAsync(serverIp, serverPort);
    Console.WriteLine($"Connected to chat server at {serverIp}:{serverPort}");

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
                    Console.WriteLine("\n" + incoming);
                    Console.ResetColor();
                    Console.Write("> ");
                }
            }
        }
        catch
        {
            Console.WriteLine("Connection lost.");
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
        Console.Write("> ");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to connect or communicate: {ex.Message}");
}