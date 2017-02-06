using Microsoft.AspNetCore.Http.Authentication;
using Nancy;

namespace TwitterMW
{
    public class AuthenticationModule : NancyModule
    {
        public AuthenticationModule()
        {
            Get("/login", async _ =>
            {
                await this.Context.GetAuthenticationManager().ChallengeAsync("Twitter", new AuthenticationProperties { RedirectUri = "/" });

                return 302;

            });


            //
            Get("/logout", async _ =>
            {
                await this.Context.GetAuthenticationManager().SignOutAsync("MyCookieMW", new AuthenticationProperties { RedirectUri = "/" });
                return "Logged out";
            });
        }
    }
}
