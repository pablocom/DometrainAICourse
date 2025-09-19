using System.ComponentModel.DataAnnotations;

namespace DometrainAiCourse.GettingStarted;

public sealed class AiOptions
{
    public const string SectionName = "AI";

    [Required]
    public required Uri BaseAddress { get; init; }
    
    [Required]
    public required string Model { get; init; }
    
    [Required]
    public required string ApiKey { get; init; }
}