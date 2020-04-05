using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using src;
using src.Repository;

namespace tests.Resources
{
    public class DatabaseAndTestServerFixture : IDisposable
    {
        public HttpClient HttpClient { get; private set; }
        public CSharpPlaygroundContext CSharpPlaygroundContext { get; private set; }
        private TestServer _testServer;

        public DatabaseAndTestServerFixture()
        {
            SetupTestServerClientAndDatabase();
            ClearDataBase();
        }

        private void SetupTestServerClientAndDatabase()
        {
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var webHostBuilder = new WebHostBuilder().UseConfiguration(configurationBuilder);
            _testServer = new TestServer(webHostBuilder.UseStartup<Startup>());
            CSharpPlaygroundContext = _testServer.Services.GetRequiredService<CSharpPlaygroundContext>();
            HttpClient = _testServer.CreateClient();
        }

        private async void ClearDataBase()
        {
            CSharpPlaygroundContext.TodoItems.RemoveRange(CSharpPlaygroundContext.TodoItems);
            await CSharpPlaygroundContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            HttpClient.Dispose();
            _testServer.Dispose();
        }
    }
}