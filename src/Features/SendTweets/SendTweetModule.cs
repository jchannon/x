using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;

namespace TwitterMW
{
    public class SendTweetModule : NancyModule
    {
        public SendTweetModule(ITweeter tweeter)
        {
            this.RequiresAuthentication();

            this.Post("/", async _ =>
            {
                var tweet = this.BindAndValidate<Tweet>();

                if (!this.ModelValidationResult.IsValid)
                {
                    return Response.AsJson(this.ModelValidationResult.FormattedErrors).WithStatusCode(422);
                }

                var status = await tweeter.SendTweet(tweet);

                return new Response().WithHeader("Location", $"{this.Request.Url.ToString()}/{status.Id}").WithStatusCode(201);
            });
        }
    }
}