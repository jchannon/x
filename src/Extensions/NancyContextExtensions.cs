using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Authentication;
using Nancy;

namespace TwitterMW
{
    public static class NancyContextExtensions
    {
        public static AuthenticationManager GetAuthenticationManager(this NancyContext context, bool throwOnNull = false)
        {
            object requestEnvironment;
            context.Items.TryGetValue(Nancy.Owin.NancyMiddleware.RequestEnvironmentKey, out requestEnvironment);
            var environment = requestEnvironment as IDictionary<string, object>;
            if (environment == null && throwOnNull)
            {
                throw new InvalidOperationException("OWIN environment not found. Is this an owin application?");
            }

            try
            {
                var httpcontext = (Microsoft.AspNetCore.Http.HttpContext)environment["Microsoft.AspNetCore.Http.HttpContext"];

                return httpcontext.Authentication;
            }
            catch (KeyNotFoundException)
            {

                try
                {
                    var defaultcontext = (Microsoft.AspNetCore.Http.DefaultHttpContext)environment["Microsoft.AspNetCore.Http.DefaultHttpContext"];

                    return defaultcontext.Authentication;
                }
                catch (KeyNotFoundException)
                {

                    return null;
                }
            }
        }
    }
}