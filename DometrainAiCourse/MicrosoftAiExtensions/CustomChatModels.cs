namespace DometrainAiCourse.MicrosoftAiExtensions;

public record CustomChatRequest(string Model, IEnumerable<CustomChatMessage> Messages);

public record CustomChatResponse(List<CustomChoice> Choices);

public record CustomChoice(CustomChatMessage Message);

public record CustomChatMessage(string? Content, CustomChatRole Role);

public enum CustomChatRole
{
    User,
    Assistant,
    System,
    Tool
}