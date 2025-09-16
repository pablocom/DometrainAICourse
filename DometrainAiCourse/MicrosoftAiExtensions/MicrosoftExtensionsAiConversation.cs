using Microsoft.Extensions.AI;

namespace DometrainAICourse.MicrosoftAiExtensions;

public class MicrosoftExtensionsAiConversation
{
    private readonly IChatClient _chatClient;

    public MicrosoftExtensionsAiConversation(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }
    
    public async Task Execute()
    {
        Console.WriteLine("Starting AI conversation using OpenAI client, write 'exit' to close the program.");

        const string greetingMessageText = "Yo what's up, how can I help you bro? :P";
        Console.WriteLine(greetingMessageText);
        
        var chatMessages = new List<ChatMessage> { new(ChatRole.Assistant, greetingMessageText) };

        var userInput = CaptureUserInput();
        while (userInput.Contains("exit", StringComparison.InvariantCultureIgnoreCase))
        {
            chatMessages.Add(new ChatMessage(ChatRole.User, userInput));

            var response = await _chatClient.GetResponseAsync(chatMessages);
            var responseText = response.Text;
    
            Console.WriteLine(responseText);
            chatMessages.Add(new ChatMessage(ChatRole.Assistant, responseText));
    
            userInput = CaptureUserInput();
        }
    }
    
    private static string CaptureUserInput()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        var input = Console.ReadLine() ?? "";
        Console.ResetColor();
        return input;
    }
}