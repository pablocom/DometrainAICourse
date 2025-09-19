using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DometrainAiCourse.GettingStarted;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace DometrainAiCourse.MicrosoftAiExtensions;

public class CustomChatClient : IChatClient
{
    private readonly IOptions<AiOptions> _aiOptions;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
    };
    
    private readonly HttpClient _httpClient;
    
    public CustomChatClient(IOptions<AiOptions> aiOptions)
    {
        _aiOptions = aiOptions;
        _httpClient = new()
        {
            BaseAddress = _aiOptions.Value.BaseAddress,
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", _aiOptions.Value.ApiKey),
                Accept = { new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json) }
            }
        };
    }
    
    public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var customChatMessages = messages.Select(x => new CustomChatMessage(x.Text, x.Role.ToCustomChatRole())).ToArray();
        var response = await CompleteChat(customChatMessages, options, cancellationToken);

        return new ChatResponse(new ChatMessage(response.Role.ToChatRole(), response.Content));
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, 
        ChatOptions? options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public object? GetService(Type serviceType, object? serviceKey = null) => throw new NotImplementedException();

    private async Task<CustomChatMessage> CompleteChat(IEnumerable<CustomChatMessage> messages, ChatOptions? options,
        CancellationToken cancellationToken = default)
    {
        var model = options?.ModelId ?? _aiOptions.Value.Model;
        var tools = (options?.Tools ?? [])
            .Select(x => new CustomTool(Type: "function", new CustomFunction(x.Name, x.Description, x.AdditionalProperties)))
            .ToArray();
        
        var request = new CustomChatRequest(model, messages.ToArray(), tools);
        var requestJson = JsonSerializer.Serialize(request, JsonSerializerOptions);
        var response = await _httpClient.PostAsync(
            "chat/completions", 
            new StringContent(requestJson, Encoding.UTF8, MediaTypeNames.Application.Json), 
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        var responseContentJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseContent = JsonSerializer.Deserialize<CustomChatResponse>(responseContentJson, JsonSerializerOptions);
        if (responseContent is null)
            throw new JsonException("Cannot deserialize custom chat response");
        
        var choice = responseContent.Choices.First();
        
        // Missing code for capturing tool invocation and executing it
        
        return choice.Message ?? throw new InvalidOperationException("Response message cannot be null");
    }
    
    public void Dispose()
    {
        _httpClient.Dispose();
    }
}