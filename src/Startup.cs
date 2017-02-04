using System;
using System.Threading.Tasks;
using CoreTweet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy.Owin;

namespace TwitterMW
{
    public class Startup
    {
        private IConfiguration Configuration;
        private readonly IHostingEnvironment environment;

        public Startup(IHostingEnvironment env)
        {
            this.environment = env;

            var builder = new ConfigurationBuilder()
                             .AddJsonFile("appsettings.json")
                             .SetBasePath(env.ContentRootPath);

            this.Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, ITweeterAuthenticator tweeterAuthenticator, Func<Tokens, ITweeter> tweeter)
        {
            loggerFactory
               .AddConsole();

            if (environment.IsEnvironment("testing"))
            {
                app.UseMiddleware<AddAuthenticatedUserMiddleware>();
            }

            app.UseCookieAuthentication(GetCookieOptions());

            app.UseTwitterAuthentication(new TwitterOptions
            {
                ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"],
                ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"],
                SignInScheme = "MyCookieMW",
                SaveTokens = true
            });

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new DemoBootstrapper(tweeterAuthenticator, tweeter)));
        }

        private CookieAuthenticationOptions GetCookieOptions()
        {
            return new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                CookieSecure = CookieSecurePolicy.SameAsRequest,
                AuthenticationScheme = "MyCookieMW",
                CookieHttpOnly = true,
                SlidingExpiration = true,
                CookieName = "MyCookie",
                LoginPath = new PathString("/login"),
                AutomaticChallenge = true
            };
        }
    }
}