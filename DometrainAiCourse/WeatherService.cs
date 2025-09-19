using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace DometrainAICourse;

public sealed class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<WeatherApiOptions> _options;

    public WeatherService(HttpClient httpClient, IOptions<WeatherApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<WeatherResponse> GetWeather(string city, CancellationToken cancellationToken)
    {
        var url = $"https://api.weatherapi.com/v1/current.json?key={_options.Value.Key}&q={city}&aqi=no";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var responseContentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var jsonDocument = await JsonDocument.ParseAsync(responseContentStream, cancellationToken: cancellationToken);

        var currentElement = jsonDocument.RootElement.GetProperty("current");
        var description = currentElement.GetProperty("condition").GetProperty("text").GetString()!;
        var temperatureInCelsius = currentElement.GetProperty("temp_c").GetDecimal();
        var humidity = currentElement.GetProperty("humidity").GetInt32();

        return new(temperatureInCelsius, humidity, description);
    }
}

public sealed class WeatherApiOptions
{
    public const string SectionName = "WeatherApi";

    [Required] 
    public required string Key { get; init; }
}

public sealed record WeatherResponse(
    decimal TemperatureInCelsius, 
    decimal HumidityPercentage, 
    string Description);