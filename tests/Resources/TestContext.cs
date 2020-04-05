using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using src;

namespace tests.Resources
{
    public class TestContext : IDisposable
    {
        public HttpClient Client { get; private set; }
        private TestServer _testServer;

        public TestContext()
        {
            SetupClient();
        }
        private void SetupClient()   
        {
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var webHostBuilder = new WebHostBuilder().UseConfiguration(configurationBuilder);
            _testServer = new TestServer(webHostBuilder.UseStartup<Startup>());
            Client  =  _testServer.CreateClient();
        }

        public void Dispose()
        { 
            // It's not called yet. I'll be here soon!
            _testServer?.Dispose();
            Client?.Dispose();
        }
    }
}