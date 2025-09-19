using System.Collections.Frozen;
using Microsoft.Extensions.AI;

namespace DometrainAiCourse.MicrosoftAiExtensions;

public record CustomChatRequest(
    string Model, 
    CustomChatMessage[] Messages, 
    CustomTool[]? Tools = null);

public record CustomTool(
    string Type,
    CustomFunction Function
);

public record CustomFunction(
    string Name,
    string? Description,
    IReadOnlyDictionary<string, object?>? Parameters = null
);

public record CustomToolCall(
    string Id,
    string Type,
    CustomFunctionCall Function
);

public record CustomFunctionCall(
    string Name,
    string Arguments
);

public record CustomChatResponse(CustomChoice[] Choices);

public record CustomChoice(CustomChatMessage Message);

public record CustomChatMessage(string? Content, CustomChatRole Role, CustomToolCall[]? ToolCalls = null);

public enum CustomChatRole
{
    User,
    Assistant,
    System,
    Tool
}

public static class CustomChatModelsExtensions
{
    private static readonly FrozenDictionary<ChatRole, CustomChatRole> MappingToCustomRole = new KeyValuePair<ChatRole, CustomChatRole>[]
    {
        new(ChatRole.Assistant, CustomChatRole.Assistant),
        new(ChatRole.User, CustomChatRole.User),
        new(ChatRole.System, CustomChatRole.System),
        new(ChatRole.Tool, CustomChatRole.Tool),
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<CustomChatRole, ChatRole> MappingToRole = MappingToCustomRole
        .Select(x => KeyValuePair.Create(x.Value, x.Key))
        .ToFrozenDictionary();

    public static CustomChatRole ToCustomChatRole(this ChatRole role) => MappingToCustomRole[role];
    public static ChatRole ToChatRole(this CustomChatRole role) => MappingToRole[role];
}