using Nancy;
using Nancy.Security;

namespace TwitterMW
{
    public class HomeModule : NancyModule
    {
        public HomeModule(ITweeter tweeter)
        {
            this.RequiresAuthentication();

            Get("/", async _ =>
            {
                var data = await tweeter.GetHomeTimeline(5);
                return Response.AsJson(data);
            });

        }
    }
}