namespace TwitterMW
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CoreTweet;

    public interface ITweeter
    {
        Task<IEnumerable<Tweet>> GetHomeTimeline(int numberofTweets);

        Task<SearchResult> Search(string searchTerm);

        Task<StatusResponse> SendTweet(SendTweetModel status);

        Task<StatusResponse> DeleteTweet(long id);

        Task<Tweet> GetTweet(long id);
    }
}