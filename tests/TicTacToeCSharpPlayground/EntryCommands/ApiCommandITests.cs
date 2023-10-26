using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Tests.Support;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.EntryCommands
{
    public class ApiCommandITests : IntegrationTestsWithDependencyInjection
    {
        private readonly string _livenessRequestUri;
        private readonly string _readinessRequestUri;

        public ApiCommandITests()
        {
            _livenessRequestUri = "/healthcheck/liveness";
            _readinessRequestUri = "/healthcheck/readiness";
        }

        [Fact]
        public void ShouldReturnOneRegistrationGivenCurrentHealthCheckConfiguration()
        {
            // Act
            var healthCheckConfiguration = Services.GetRequiredService<IOptions<HealthCheckServiceOptions>>();
            var registration = healthCheckConfiguration.Value.Registrations;
            // Assert
            registration.Count().Should().Be(1);
            registration.Select(x => x.Name).Should().Contain("npgsql");
        }

        [Fact]
        public async Task ShouldConsultHealthCheckEndpoints()
        {
            // Act
            var responseReadiness = await Client.GetAsync(_readinessRequestUri);
            var responseLiveness = await Client.GetAsync(_livenessRequestUri);
            // Assert
            responseLiveness.StatusCode.Should().Be(HttpStatusCode.OK);
            responseReadiness.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
