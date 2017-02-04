using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CoreTweet;
using CoreTweet.Core;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using TwitterMW;
using Xunit;

namespace TwitterMWTests
{
    public class DeleteTweetTests
    {
        private TestServer server;

        private HttpClient client;

        public DeleteTweetTests()
        {
            var tweeterAuthenticator = A.Fake<ITweeterAuthenticator>();

            var tweeter = A.Fake<ITweeter>();
            A.CallTo(() => tweeter.DeleteTweet(123)).Returns(new StatusResponse { Id = 123 });
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
            var response = await client.DeleteAsync("/123");

            //Then
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("http://localhost/login", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task Should_delete_tweet()
        {
            //Given, When
            var response = await client.DeleteAsync("/123");

            //Then
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Should_catch_exception__and_return_no_content_on_invalid_tweet_id()
        {
            //Given
            var tweeterAuthenticator = A.Fake<ITweeterAuthenticator>();

            var tweeter = A.Fake<ITweeter>();
            A.CallTo(() => tweeter.DeleteTweet(999)).Throws(new TwitterException());//This doesnt compile as TwitterException has a private ctor
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
            //When
            var response = await client.DeleteAsync("/999");

            //Then
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}