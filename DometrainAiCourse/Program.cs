using System.ClientModel;
using DometrainAICourse;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

var chatClient = CreateChatClient();

Console.WriteLine("Starting AI conversation, write 'exit' to close the program.");

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

return;

ChatClient CreateChatClient()
{
    var configuration = new ConfigurationBuilder()
        .AddUserSecrets<IAssemblyMarker>()
        .Build();

    var options = new AiOptions();
    configuration.Bind(AiOptions.SectionName, options);

    return new ChatClient(
        model: "llama-3.1-8b-instant",
        credential: new ApiKeyCredential(options.ApiKey),
        options: new OpenAIClientOptions { Endpoint = new Uri("https://api.groq.com/openai/v1") }
    );
}

string CaptureUserInput()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    var input = Console.ReadLine() ?? "";
    Console.ResetColor();
    return input;
}