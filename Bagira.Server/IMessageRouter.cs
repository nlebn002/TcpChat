using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagira.Server
{
    public interface IMessageRouter
    {
        Task RouteMessageAsync(string sender, string message, ChatClientHandler senderClient, List<ChatClientHandler> allClients);
    }
}
