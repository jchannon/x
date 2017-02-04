using System;
using CoreTweet;
using Microsoft.AspNetCore.Http.Authentication;

namespace TwitterMW
{
    public class TweeterAuthenticator : ITweeterAuthenticator
    {
        public Tokens Authorise(string consumerKey, string consumerSecret, AuthenticationProperties properties)
        {
            var tokens = Tokens.Create(consumerKey, consumerSecret, properties.Items[".Token.access_token"], properties.Items[".Token.access_token_secret"]);

            tokens.Search.TweetsAsync("", "en");

            return tokens;
        }


    }
}