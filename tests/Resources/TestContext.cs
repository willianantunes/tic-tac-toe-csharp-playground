using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
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
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
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