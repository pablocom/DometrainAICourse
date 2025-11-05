using DometrainAICourse;
using Microsoft.Extensions.AI;

namespace DometrainAiCourse.MicrosoftAiExtensions;

public sealed class ToolDefinitionsProvider
{
    private readonly WeatherService _weatherService;

    public ToolDefinitionsProvider(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }
    
    public IEnumerable<AITool> GetToolDefinitions()
    {
        var getWeatherMethodInfo = typeof(WeatherService)
            .GetMethod(nameof(WeatherService.GetWeather), [typeof(string), typeof(CancellationToken)])!;

        yield return AIFunctionFactory.Create(
            getWeatherMethodInfo, 
            _weatherService,
            new AIFunctionFactoryOptions
            {
                Name = "get_weather",
                Description = "Gets the current weather of a city"
            }
        );
    }    
}