using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TwitterMW;
using Xunit;

namespace TwitterMWTests
{
    public class SendTweetsTests
    {
        private TestServer server;

        private HttpClient client;

        private SendTweetModel tweet;

        public SendTweetsTests()
        {
            var tweeterAuthenticator = A.Fake<ITweeterAuthenticator>();

            var tweeter = A.Fake<ITweeter>();
            tweet = new SendTweetModel() { Message = "Who's awesome? You're awesome!" };
            A.CallTo(() => tweeter.SendTweet(A<SendTweetModel>.That.Matches(x => x.Message == tweet.Message))).Returns(new StatusResponse() { Id = 1 });
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
            var response = await client.PostAsync("/", new StringContent("tweet"));

            //Then
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("http://localhost/login", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task Should_return_422_on_invalid_body()
        {
            //Given,When
            var response = await client.PostAsync("/", new StringContent("tweet"));
            var body = await response.Content.ReadAsStringAsync();

            //Then
            Assert.Contains("error", body);
            Assert.Equal(422, (int)response.StatusCode);
        }

        [Fact]
        public async Task Should_return_201_and_location_header()
        {
            //Given,When
            var response = await client.PostAsync("/", new StringContent(JsonConvert.SerializeObject(tweet), Encoding.UTF8, "application/json"));

            //Then
            Assert.Equal(201, (int)response.StatusCode);
            Assert.Equal("http://localhost/1", response.Headers.Location.ToString());
        }
    }
}