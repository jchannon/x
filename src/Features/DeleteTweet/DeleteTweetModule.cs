namespace TwitterMW
{
    using Nancy;
    using Nancy.Security;
    using System;

    public class DeleteTweetModule : NancyModule
    {
        public DeleteTweetModule(ITweeter tweeter)
        {
            this.RequiresAuthentication();

            this.Delete("/{id:long}", async args =>
            {
                //This try/catch could go in the tweeter implementation
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