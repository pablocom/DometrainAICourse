using System.ComponentModel.DataAnnotations;

namespace DometrainAICourse;

public sealed class AIOptions
{
    public const string SectionName = "AI";

    [Required]
    public required Uri ProviderEndpoint { get; init; }
    
    [Required]
    public required string Model { get; init; }
    
    [Required]
    public required string ApiKey { get; init; }
}