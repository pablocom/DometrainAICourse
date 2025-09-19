using System.ClientModel;
using DometrainAICourse;
using DometrainAiCourse.GettingStarted;
using DometrainAiCourse.MicrosoftAiExtensions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenAI;

var builder = Host.CreateApplicationBuilder();

builder.Configuration.AddUserSecrets<IAssemblyMarker>();

builder.Services.AddValidatedOptions<AiOptions>(AiOptions.SectionName);
builder.Services.AddValidatedOptions<WeatherApiOptions>(WeatherApiOptions.SectionName);

builder.Services.AddChatClient(sp =>
{
    var options = sp.GetRequiredService<IOptions<AiOptions>>();
    return new OpenAI.Chat.ChatClient(options.Value.Model, new ApiKeyCredential(options.Value.ApiKey), new OpenAIClientOptions
    {
        Endpoint = options.Value.BaseAddress
    }).AsIChatClient();
});

builder.Services.AddSingleton<AiClientConversation>();
builder.Services.AddSingleton<ToolDefinitionsProvider>();
builder.Services.AddHostedService<AiClientConversationHostedService>();
builder.Services.AddHttpClient<WeatherService>();

using var app = builder.Build();

await app.RunAsync();
