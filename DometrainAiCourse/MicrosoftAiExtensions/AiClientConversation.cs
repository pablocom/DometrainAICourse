using Microsoft.Extensions.AI;

namespace DometrainAiCourse.MicrosoftAiExtensions;

public class AiClientConversation
{
    private readonly IChatClient _chatClient;

    public AiClientConversation(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }
    
    public async Task ExecuteConversationLoop(CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.Assistant, "Glad to see you sir Stark, how can I help you?")
        };

        Console.WriteLine(messages.First().Text);

        var userInput = CaptureUserInput();
        while (userInput != "exit")
        {
            messages.Add(new ChatMessage(ChatRole.User, userInput));

            var chatResponse = await _chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
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