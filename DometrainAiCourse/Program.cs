using DometrainAICourse;
using DometrainAiCourse.MicrosoftAiExtensions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();
var configuration = new ConfigurationManager();

configuration.AddUserSecrets<IAssemblyMarker>();

services.AddLogging(x => x.AddConsole().SetMinimumLevel(LogLevel.Information))
    .AddSingleton(LoggerFactory.Create(x => x.AddConsole().SetMinimumLevel(LogLevel.Information)))
    .AddSingleton<IConfiguration>(configuration)
    .AddSingleton<IChatClient, CustomChatClient>()
    .AddSingleton<AiClientConversation>();

var serviceProvider = services.BuildServiceProvider();

var conversation = serviceProvider.GetRequiredService<AiClientConversation>();

await conversation.ExecuteConversationLoop();
