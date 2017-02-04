using CoreTweet;
using Microsoft.AspNetCore.Http.Authentication;

namespace TwitterMW
{
    public interface ITweeterAuthenticator
    {
        Tokens Authorise(string consumerKey, string consumerSecret, AuthenticationProperties properties);
    }
}