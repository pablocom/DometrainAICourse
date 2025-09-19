using Microsoft.Extensions.DependencyInjection;

namespace DometrainAICourse;

public static class OptionsExtensions
{
    public static void AddValidatedOptions<TOptions>(this IServiceCollection services, string sectionName) 
        where TOptions : class
    {
        services.AddOptions<TOptions>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}