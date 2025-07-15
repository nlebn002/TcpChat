using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bagira.Server
{
    public class MessageRouter : IMessageRouter
    {
        private readonly ILetterCounter _counter;
        private readonly ILogger<MessageRouter> _logger;

        public MessageRouter(ILetterCounter counter, ILogger<MessageRouter> logger)
        {
            _counter = counter;
            _logger = logger;
        }

        public async Task RouteMessageAsync(string sender, string message, ChatClientHandler senderClient, List<ChatClientHandler> allClients)
        {
            var timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string fullMessage = $"{timestamp}, {sender} – {message}";

            _counter.AddLetters(message);
            var stats = _counter.GetStats();

            if (message.StartsWith("To:"))
            {
                var split = message.Split("–", 2);
                var namePart = split[0].Substring(3).Trim();
                var content = split.Length > 1 ? split[1].Trim() : "";
                var target = allClients.FirstOrDefault(c => c.NickName == namePart);
                if (target != null)
                {
                    await target.SendAsync(fullMessage);
                    await senderClient.SendAsync($"(Private to {namePart}): {content}");
                }
                else
                {
                    await senderClient.SendAsync("User not found.");
                }
            }
            else
            {
                foreach (var client in allClients)
                {
                    await client.SendAsync(fullMessage);
                }
            }

            _logger.LogInformation($"[Stats] {string.Join(", ", stats.Select(kvp => $"{kvp.Key}:{kvp.Value}"))}");
        }
    }
}
