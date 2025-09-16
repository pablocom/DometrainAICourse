using System.ClientModel;
using DometrainAICourse;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

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

        var options = new AiOptions();
        configuration.Bind(AiOptions.SectionName, options);

        return new ChatClient(
            model: Constants.AiModel,
            credential: new ApiKeyCredential(options.ApiKey),
            options: new OpenAIClientOptions { Endpoint = Constants.AiProviderBaseAddress }
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