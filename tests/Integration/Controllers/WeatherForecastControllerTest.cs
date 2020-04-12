using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using src.Domain;
using Xunit;

namespace tests.Integration.Controllers
{
    public class WeatherForecastControllerTest : IClassFixture<WebApplicationFactory<src.Startup>>
    {
        private HttpClient _httpClient;

        public WeatherForecastControllerTest(WebApplicationFactory<src.Startup> factory)
        {
            _httpClient = factory.CreateClient();
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
