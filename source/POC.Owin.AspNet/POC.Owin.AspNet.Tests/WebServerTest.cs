using Microsoft.Owin.Testing;
using System;
using System.Net.Http;
using Xunit;

namespace POC.Owin.AspNet.Tests
{
    public class WebServerTest : IDisposable
    {
        private readonly TestServer _server;

        public WebServerTest()
        {
            _server = TestServer.Create<Startup>();
        }

        public void Dispose()
        {
            _server.Dispose();
        }


        [Fact]
        public void Test()
        {
            var response = _server.HttpClient.GetAsync("/api/test").Result;
            var result = response.Content.ReadAsAsync<int>().Result;
            Assert.Equal(1, result);
        }
    }
}
