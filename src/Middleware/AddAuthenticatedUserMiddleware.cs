using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;

namespace TwitterMW
{
    public class AddAuthenticatedUserMiddleware
    {
        private readonly RequestDelegate next;

        public AddAuthenticatedUserMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var claims = new List<Claim>(new[]
                        {
                            new Claim(ClaimTypes.Name, "BillyBob")
                        });

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "MyCookieMW"));
            context.User = claimsPrincipal;

            //var info = await context.Authentication.GetAuthenticateInfoAsync("Twitter");
            var props = new AuthenticationProperties();
            props.StoreTokens(new[] {
                        new AuthenticationToken { Name = "access_token", Value = "49584881-Xh7f7x4irFiSuJfAkZDQzHXPhal480Xht1o046CIZ" },
                        new AuthenticationToken { Name = "access_token_secret", Value = "YTeDOoDM330OHmc7k5Svk6tO3m0EVGVDjj5iMpcmC1m2J" }
                    });

            // var handler = HttpAuthenticationFeature.Handler;
            // var context = new AuthenticateContext("Twitter");
            // if (handler != null)
            // {
            //     await handler.AuthenticateAsync(context);
            //}

            //await context.Authentication.SignInAsync("MyCookieMW", claimsPrincipal, props);

            await next(context);
        }
    }
}