using Bagira.Shared.Abstractions;

namespace Bagira.Shared;

public class ConsoleMessageFormatter : IMessageFormatter
{
    public string FormatMessage(string message)
    {
        if (!includeTimestamp)
            return message;

        var timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        return $"[{timestamp}] {message}";
    }
}
