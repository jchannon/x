using System;
using CoreTweet;
using Nancy;
using Nancy.Configuration;
using Nancy.Security;
using Nancy.TinyIoc;

namespace TwitterMW
{
    public class DemoBootstrapper : DefaultNancyBootstrapper
    {
        private readonly ITweeterAuthenticator tweeterAuthenticator;
        private readonly Func<Tokens, ITweeter> tweeter;

        public DemoBootstrapper(ITweeterAuthenticator tweeterAuthenticator, Func<Tokens, ITweeter> tweeter)
        {
            this.tweeter = tweeter;
            this.tweeterAuthenticator = tweeterAuthenticator;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<ITweeterAuthenticator>(this.tweeterAuthenticator); // 😉
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            Tokens tokens = null;
            if (context.CurrentUser.IsAuthenticated())
            {
                var tweeterAuthenticator = container.Resolve<ITweeterAuthenticator>();
                var info = context.GetAuthenticationManager().GetAuthenticateInfoAsync("Twitter").Result;

                tokens = tweeterAuthenticator.Authorise("4sOcfh5LWkJlzFHO0PHuTqAHi", "DllzqiX7fIyJKMIs4n20Z5PXzpXJgJ5NrtqjKXjThE6bLBeZK9", info.Properties);
            }

            var result = this.tweeter(tokens);
            container.Register<ITweeter>(result);
        }

        public override void Configure(INancyEnvironment env)
        {
            env.Tracing(false, true);
        }
    }
}