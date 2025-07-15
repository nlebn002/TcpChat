namespace Bagira.Shared.Abstractions;

public interface IChatSender
{
    Task OpenSendLoopAsync();
}
