using Microsoft.Extensions.Hosting;

namespace DometrainAiCourse.MicrosoftAiExtensions;

public sealed class AiClientConversationHostedService(AiClientConversation conversation, IHost host) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await conversation.ExecuteConversationLoop(stoppingToken);
        await host.StopAsync(stoppingToken);
    }
}
