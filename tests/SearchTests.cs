using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CoreTweet;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TwitterMW;
using Xunit;

namespace TwitterMWTests
{
    public class SearchTests
    {
        private TestServer server;

        private HttpClient client;

        public SearchTests()
        {
            var tweeterAuthenticator = A.Fake<ITweeterAuthenticator>();

            var tweeter = A.Fake<ITweeter>();

            //SearchResult takes no constructor to set the value
            string json = "{\"statuses\":[{\"Text\":\"MVC Sucks, try #NancyFX\", \"Id\":1}]}";
            SearchResult sr = JToken.Parse(json).SelectToken("").ToObject<SearchResult>();

            A.CallTo(() => tweeter.Search("mvcsucks")).Returns(sr);
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
        public async Task Should_return_search_results()
        {
            //Given,When
            var response = await client.GetAsync("/search/mvcsucks");
            var body = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<List<Status>>(body);

            //Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, model.Count);
            Assert.Equal(1, model.First().Id);
            Assert.Equal("MVC Sucks, try #NancyFX", model.First().Text);
        }

        [Fact]
        public async Task Should_return_redirect_to_login_when_not_authenticated()
        {
            //Given
            server = new TestServer(new WebHostBuilder()

               .UseStartup<Startup>()
               .ConfigureServices(serv =>
               {
                   serv.AddSingleton<Func<Tokens, ITweeter>>((_) => A.Fake<ITweeter>());
                   serv.AddSingleton<ITweeterAuthenticator>(A.Fake<ITweeterAuthenticator>());
               })
               );

            client = server.CreateClient();
            //When
            var response = await client.GetAsync("/search/pulpfiction");
            var body = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<List<Status>>(body);

            //Then
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("http://localhost/login", response.Headers.Location.ToString());
        }
    }
}