using System.Net.Sockets;
using System.Text;

Console.Write("Enter your name: ");
string name = Console.ReadLine()?.Trim() ?? "Anonymous";

var client = new TcpClient();
try
{
    await client.ConnectAsync("127.0.0.1", 5000);
    var stream = client.GetStream();
    var receiveTask = Task.Run(async () =>
    {
        byte[] buffer = new byte[4096];
        while (true)
        {
            int bytesRead = await stream.ReadAsync(buffer);
            if (bytesRead == 0) break;
            Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, bytesRead));
        }
    });

    await stream.WriteAsync(Encoding.UTF8.GetBytes(name + Environment.NewLine));

    while (true)
    {
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) continue;
        await stream.WriteAsync(Encoding.UTF8.GetBytes(input + Environment.NewLine));
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Could not connect: {ex.Message}");
}