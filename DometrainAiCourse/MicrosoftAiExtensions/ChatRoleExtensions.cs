using System.Collections.Frozen;
using Microsoft.Extensions.AI;

namespace DometrainAiCourse.MicrosoftAiExtensions;

public static class ChatRoleExtensions
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