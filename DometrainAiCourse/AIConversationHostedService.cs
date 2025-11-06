using Microsoft.Extensions.Hosting;

namespace DometrainAICourse;

public sealed class AIConversationHostedService(AIConversation conversation, IHost host) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await conversation.ConversationLoop(stoppingToken);
        await host.StopAsync(stoppingToken);
    }
}
