namespace Bagira.Shared;

public static class Utils
{
    public static string FormatMessage(string msg) =>
         $"{DateTime.UtcNow.ToString("dd/mm/year hh:mm:ss")}{msg}";

    public const string DateFormat = "dd/MM/yyyy HH:mm:ss";
}
