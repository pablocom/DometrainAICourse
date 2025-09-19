using System.ClientModel;
using DometrainAICourse;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using ChatMessage = OpenAI.Chat.ChatMessage;

namespace DometrainAiCourse.GettingStarted;

public static class OpenAiClientConversation
{
    public static async Task Execute()
    {
        var chatClient = CreateChatClient();

        Console.WriteLine("Starting AI conversation using OpenAI client, write 'exit' to close the program.");

        var greetingMessage = new AssistantChatMessage("Yo what's up, how can I help you bro? :P");
        Console.WriteLine(greetingMessage.Content[0].Text);

        List<ChatMessage> messages = [greetingMessage];

        var userInput = CaptureUserInput();
        while (userInput != "exit")
        {
            messages.Add(new UserChatMessage(userInput));

            var chatCompletion = await chatClient.CompleteChatAsync(messages);
            var response = chatCompletion.Value.Content[0].Text;
    
            Console.WriteLine(response);
            messages.Add(new AssistantChatMessage(response));
    
            userInput = CaptureUserInput();
        }

    }
    
    private static ChatClient CreateChatClient()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<IAssemblyMarker>()
            .Build();

        var options = configuration.GetValue<AiOptions>(AiOptions.SectionName);
        if (options is null)
            throw new InvalidOperationException("Cannot read AI provider options");

        return new ChatClient(
            model: options.Model,
            credential: new ApiKeyCredential(options.ApiKey),
            options: new OpenAIClientOptions { Endpoint = options.BaseAddress }
        );
    }

    private static string CaptureUserInput()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        var input = Console.ReadLine() ?? "";
        Console.ResetColor();
        return input;
    }
}