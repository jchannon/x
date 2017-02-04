
using System.Threading.Tasks;
using CoreTweet;
using CoreTweet.Core;

namespace TwitterMW
{
    public class Tweeter : ITweeter
    {
        private readonly Tokens tokens;

        public Tweeter(Tokens tokens)
        {
            this.tokens = tokens;
        }

        public async Task<ListedResponse<Status>> GetHomeTimeline(int numberofTweets)
        {
            return await this.tokens.Statuses.HomeTimelineAsync(count: numberofTweets);
        }

        public async Task<SearchResult> Search(string searchTerm)
        {
            return await this.tokens.Search.TweetsAsync(q: searchTerm, lang: "en", count: 5);
        }

        public async Task<StatusResponse> SendTweet(Tweet status)
        {
            return await this.tokens.Statuses.UpdateAsync(status.Message);
        }

        public async Task<StatusResponse> DeleteTweet(long id)
        {
            return await this.tokens.Statuses.DestroyAsync(id);
        }
    }
}