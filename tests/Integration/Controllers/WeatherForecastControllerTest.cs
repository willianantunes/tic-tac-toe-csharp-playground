using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using src.Domain;
using tests.Resources;
using Xunit;

namespace tests.Integration.Controllers
{
    public class WeatherForecastControllerTest : IClassFixture<DatabaseAndTestServerFixture>
    {
        private HttpClient _httpClient;

        public WeatherForecastControllerTest(DatabaseAndTestServerFixture testServerFixture)
        {
            _httpClient = testServerFixture.HttpClient;
        }

        [Fact(DisplayName ="Should consult [controller] placeholder and receive 5 weather forecasts")]
        public async Task ShouldConsultAndReceive5WeatherForecasts()
        {
            var response = await _httpClient.GetAsync("/WeatherForecast");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            var weatherForecasts = JsonConvert.DeserializeObject<List<WeatherForecast>>(content);

            weatherForecasts.Should().HaveCount(5);
            weatherForecasts.ForEach(weatherForecast =>
            {
                weatherForecast.Date.Should().BeAfter(DateTime.Now);
                weatherForecast.Summary.Should().NotBeEmpty();
                weatherForecast.TemperatureC.Should().BeInRange(-22, 55);
                var expectedTempFValue = 32 + (int) (weatherForecast.TemperatureC / 0.5556);
                weatherForecast.TemperatureF.Should().Be(expectedTempFValue);
            });
        }
    }
}