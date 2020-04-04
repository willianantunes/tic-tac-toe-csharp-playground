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
    public class WeatherForecastControllerTest
    {
        private readonly HttpClient _httpClient;

        public WeatherForecastControllerTest()
        {
            var testContext = new TestContext();
            _httpClient = testContext.Client;
        }

        [Fact(DisplayName ="Should consult [controller] placeholder and receive 5 weather forecasts")]
        public async Task ShouldConsultAndReceive5WeatherForecasts()
        {
            var response = await _httpClient.GetAsync("/WeatherForecast");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            var weatherForecasts = JsonConvert.DeserializeObject<List<WeatherForecast>>(content);

            weatherForecasts.Should().HaveCount(5);
        }
    }
}