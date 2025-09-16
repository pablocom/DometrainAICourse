using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using DometrainAiCourse.GettingStarted;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

namespace DometrainAiCourse.MicrosoftAiExtensions;

public class CustomChatClient : IChatClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
    };
    
    private readonly HttpClient _httpClient;
    
    public CustomChatClient(IConfiguration configuration)
    {
        _httpClient = new()
        {
            BaseAddress = Constants.AiProviderBaseAddress,
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", configuration.GetValue<string>("AiOptions:ApiKey")),
                Accept = { new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json) }
            }
        };
    }
    
    public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? _ = null,
        CancellationToken cancellationToken = default)
    {
        var customChatMessages = messages.Select(x => new CustomChatMessage(x.Text, x.Role.ToCustomChatRole())).ToArray();
        var response = await CompleteChat(customChatMessages, cancellationToken);

        return new ChatResponse(new ChatMessage(response.Role.ToChatRole(), response.Content));
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, 
        ChatOptions? options = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public object? GetService(Type serviceType, object? serviceKey = null) => throw new NotImplementedException();

    private async Task<CustomChatMessage> CompleteChat(IEnumerable<CustomChatMessage> messages, CancellationToken cancellationToken = default)
    {
        var request = new CustomChatRequest(Constants.AiModel, messages);
        var response = await _httpClient.PostAsJsonAsync("chat/completions", request, JsonSerializerOptions, cancellationToken);

        response.EnsureSuccessStatusCode();
        
        var responseContent = await response.Content.ReadFromJsonAsync<CustomChatResponse>(JsonSerializerOptions, cancellationToken);
        if (responseContent is null)
            throw new JsonException("Cannot deserialize custom chat response");

        var choice = responseContent.Choices.First();
        return choice.Message ?? throw new InvalidOperationException("Response message cannot be null");
    }
    
    public void Dispose()
    {
        _httpClient.Dispose();
    }
}