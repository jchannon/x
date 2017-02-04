namespace TwitterMW
{
    using System.Threading.Tasks;
    using CoreTweet;
    using CoreTweet.Core;

    public interface ITweeter
    {
        Task<ListedResponse<Status>> GetHomeTimeline(int numberofTweets);

        Task<SearchResult> Search(string searchTerm);

        Task<StatusResponse> SendTweet(Tweet status);

        Task<StatusResponse> DeleteTweet(long id);
    }
}