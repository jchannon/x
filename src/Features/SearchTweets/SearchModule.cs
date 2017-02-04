using Nancy;
using Nancy.Security;

namespace TwitterMW
{
    public class SearchModule : NancyModule
    {
        public SearchModule(ITweeter tweeter)
        {
            this.RequiresAuthentication();

            Get("/search/{keyword}", async args =>
            {
                var data = await tweeter.Search((string)args.keyword);
                return Response.AsJson(data);
            });
        }
    }
}