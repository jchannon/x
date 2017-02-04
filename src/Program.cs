using System;
using System.IO;
using CoreTweet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace TwitterMW
{
    class Program
    {
        static void Main(string[] args)
        {

            var host = new WebHostBuilder()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .UseKestrel()
               .UseStartup<Startup>()
               .ConfigureServices(services =>
               {
                   services.AddSingleton<ITweeterAuthenticator>(new TweeterAuthenticator());
                   services.AddSingleton<Func<Tokens, ITweeter>>((tokens) => new Tweeter(tokens));
               })
               .Build();

            host.Run();

        }
    }
}
