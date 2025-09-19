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
        var getWeatherFunction = typeof(WeatherService)
            .GetMethod(nameof(WeatherService.GetWeather), [typeof(string), typeof(CancellationToken)])!;

        yield return AIFunctionFactory.Create(
            getWeatherFunction, 
            _weatherService,
            new AIFunctionFactoryOptions
            {
                Name = "get_weather",
                Description = "Get the current weather of a specified city"
            }
        );
    }    
}