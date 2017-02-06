using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using TwitterMW;
using Xunit;
using FakeItEasy;
using CoreTweet;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitterMWTests
{
    public class HomeTests
    {
        private TestServer server;

        private HttpClient client;

        public HomeTests()
        {
            var tweeterAuthenticator = A.Fake<ITweeterAuthenticator>();

            var tweeter = A.Fake<ITweeter>();
            A.CallTo(() => tweeter.GetHomeTimeline(20)).Returns(new[] { new Tweet() { Body = "Hi" } });
            A.CallTo(() => tweeter.GetTweet(1)).Returns(new Tweet() { Body = "Good day!" });

            Func<Tokens, ITweeter> func = (_) => tweeter;

            server = new TestServer(new WebHostBuilder()

                .UseStartup<Startup>()
                .UseEnvironment("testing")
                .ConfigureServices(serv =>
                {
                    serv.AddSingleton<Func<Tokens, ITweeter>>(func);
                    serv.AddSingleton<ITweeterAuthenticator>(tweeterAuthenticator);
                })
                );

            client = server.CreateClient();
        }

        [Fact]
        public async Task Should_return_list_of_tweets()
        {
            //Given,When
            var response = await client.GetAsync("/");
            var body = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<List<Tweet>>(body);

            //Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, model.Count);
        }

        [Fact]
        public async Task Should_return_tweets_by_id()
        {
            //Given,When
            var response = await client.GetAsync("/1");
            var body = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<Tweet>(body);

            //Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Good day!", model.Body);
        }

        [Fact]
        public async Task Should_return_redirect_to_login_when_not_authenticated()
        {
            //Given
            var server = new TestServer(new WebHostBuilder()

               .UseStartup<Startup>()
               .ConfigureServices(serv =>
               {
                   serv.AddSingleton<Func<Tokens, ITweeter>>((_) => A.Fake<ITweeter>());
                   serv.AddSingleton<ITweeterAuthenticator>(A.Fake<ITweeterAuthenticator>());
               })
               );

            var client = server.CreateClient();
            //When
            var response = await client.GetAsync("/");

            //Then
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("http://localhost/login", response.Headers.Location.ToString());
        }
    }
}