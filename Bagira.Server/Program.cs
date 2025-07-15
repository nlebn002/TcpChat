
using Bagira.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IChatTransport, TcpChatTransport>();
        services.AddSingleton<IMessageRouter, MessageRouter>();
        services.AddSingleton<ILetterCounter, LetterCounter>();
        services.AddHostedService<ChatServer>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build()
    .Run();