namespace DometrainAiCourse.MicrosoftAiExtensions;

public record CustomChatRequest(string Model, IEnumerable<CustomChatMessage> Messages);