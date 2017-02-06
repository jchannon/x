using System;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace TwitterMW
{
    public class TweetsModule : NancyModule
    {
        public TweetsModule(ITweeter tweeter)
        {
            this.RequiresAuthentication();

            this.Get("/", async _ =>
            {
                var data = await tweeter.GetHomeTimeline(20);
                return Response.AsJson(data);
            });

            this.Post("/", async _ =>
            {
                var tweet = this.BindAndValidate<SendTweetModel>();

                if (!this.ModelValidationResult.IsValid)
                {
                    return Response.AsJson(this.ModelValidationResult.FormattedErrors).WithStatusCode(422);
                }

                var status = await tweeter.SendTweet(tweet);

                return new Response().WithHeader("Location", $"{this.Request.Url.ToString()}/{status.Id}").WithStatusCode(201);
            });

            this.Get("/search/{keyword}", async args =>
            {
                var data = await tweeter.Search((string)args.keyword);
                return Response.AsJson(data);
            });

            this.Get("/{id:long}", async args =>
            {
                //827496791536967682
                var data = await tweeter.GetTweet((long)args.id);
                return Response.AsJson(data);
            });

            this.Delete("/{id:long}", async args =>
            {
                try
                {
                    var data = await tweeter.DeleteTweet((long)args.id);
                }
                catch (InvalidOperationException)
                { }

                return 204;
            });
        }
    }
}