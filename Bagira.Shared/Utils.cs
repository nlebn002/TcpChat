namespace Bagira.Shared;

public static class Utils
{
    /// <summary>
    /// Formats a message with the current UTC timestamp.
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static string FormatWithTimestamp(string msg, bool isLocal = false)
    {
        var dateTime = isLocal ? DateTime.Now : DateTime.UtcNow;
        return $"{dateTime}, {msg}";
    }


    //All the consts should be in some configuration file (if its not, I didnt have time, or forgot about that :( )

    /// <summary>
    /// Default date format used in the application.
    /// </summary>
    public const string DateFormat = "dd/MM/yyyy HH:mm:ss";

    /// <summary>
    /// Default ip for the chat server.
    /// </summary>
    public const string ServerIP = "127.0.0.1";

    /// <summary>
    /// Default port for the chat server.
    /// </summary>
    public const int ServerPort = 5000;
}
