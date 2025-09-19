using DometrainAiCourse.GettingStarted;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace DometrainAiCourse.MicrosoftAiExtensions;

public class AiClientConversation
{
    private readonly IChatClient _chatClient;
    private readonly IOptions<AiOptions> _aiOptions;
    private readonly ToolDefinitionsProvider _toolDefinitionsProvider;

    public AiClientConversation(IChatClient chatClient, IOptions<AiOptions> aiOptions, ToolDefinitionsProvider toolDefinitionsProvider)
    {
        _chatClient = chatClient;
        _aiOptions = aiOptions;
        _toolDefinitionsProvider = toolDefinitionsProvider;
    }
    
    public async Task ExecuteConversationLoop(CancellationToken cancellationToken = default)
    {
        var chatOptions = new ChatOptions
        {
            ModelId = _aiOptions.Value.Model,
            Temperature = 1,
            MaxOutputTokens = 5000,
            Tools = _toolDefinitionsProvider.GetToolDefinitions().ToList()
        };
        
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful AI assistant, that uses defined tools when it's appropriate"),
            new(ChatRole.Assistant, "Glad to see you sir Stark, how can I help you?")
        };

        Console.WriteLine(messages.Last().Text);

        var userInput = CaptureUserInput();
        while (userInput != "exit")
        {
            messages.Add(new ChatMessage(ChatRole.User, userInput));

            var chatResponse = await _chatClient.GetResponseAsync(messages, chatOptions, cancellationToken);
            var chatResponseMessage = chatResponse.Messages.First();
            
            Console.WriteLine(chatResponse.Text);
    
            messages.Add(chatResponseMessage);
            
            userInput = CaptureUserInput();
        }
        
        Console.WriteLine("Exiting conversation...");
    }

    private static string CaptureUserInput()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        var input = Console.ReadLine() ?? "";
        Console.ResetColor();
        return input;
    }
}