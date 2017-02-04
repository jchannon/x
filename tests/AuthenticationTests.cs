using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using TwitterMW;
using System.Net;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using System;
using CoreTweet;

namespace TwitterMWTests
{
    public class AuthenticationTests
    {
        private readonly TestServer server;

        private readonly HttpClient client;

        public AuthenticationTests()
        {
            server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(serv =>
                {
                    serv.AddSingleton<Func<Tokens, ITweeter>>((_) => A.Fake<ITweeter>());
                    serv.AddSingleton<ITweeterAuthenticator>(A.Fake<ITweeterAuthenticator>());
                })
                );

            client = server.CreateClient();
        }

        [Fact]
        public async Task Should_return_logged_out_body()
        {
            //Give,When
            var response = await client.GetAsync("/logout");
            var body = await response.Content.ReadAsStringAsync();

            //Then
            Assert.Equal("Logged out", body);
        }

        [Fact]
        public async Task Should_return_redirect_on_login()
        {
            var response = await client.GetAsync("/login");
            Assert.Contains("twitter.com", response.Headers.Location.ToString());
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
    }
}