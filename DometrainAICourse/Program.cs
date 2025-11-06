using System.ClientModel;
using DometrainAICourse;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenAI;

var builder = Host.CreateApplicationBuilder();

builder.Configuration.AddUserSecrets<IAssemblyMarker>();

builder.Services.AddOptionsWithValidation<AIOptions>(AIOptions.SectionName);
builder.Services.AddOptionsWithValidation<WeatherApiOptions>(WeatherApiOptions.SectionName);
builder.Services.AddSingleton<AIConversation>();
builder.Services.AddSingleton<ToolDefinitionsProvider>();
builder.Services.AddHostedService<AIConversationHostedService>();
builder.Services.AddHttpClient<WeatherService>().RemoveAllLoggers();

builder.Services.AddChatClient(sp =>
    {
        var options = sp.GetRequiredService<IOptions<AIOptions>>();
        var client = new OpenAI.Chat.ChatClient(
            options.Value.Model, 
            new ApiKeyCredential(options.Value.ApiKey),
            new OpenAIClientOptions { Endpoint = options.Value.ProviderEndpoint }
        );
        return client.AsIChatClient();
    })
    .UseFunctionInvocation();

using var app = builder.Build();

await app.RunAsync();