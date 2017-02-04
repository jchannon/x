namespace TwitterMW
{
    using CoreTweet;
    using Nancy;
    using Nancy.Security;

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
                catch (TwitterException)
                { }

                return 204;
            });
        }
    }
}