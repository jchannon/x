
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreTweet;

namespace TwitterMW
{
    public class Tweeter : ITweeter
    {
        private readonly Tokens tokens;

        public Tweeter(Tokens tokens)
        {
            this.tokens = tokens;
        }

        public async Task<IEnumerable<Tweet>> GetHomeTimeline(int numberofTweets)
        {
            var data = await this.tokens.Statuses.HomeTimelineAsync(count: numberofTweets);
            var model = data.Select(x => new Tweet() { CreatedAt = x.CreatedAt, Body = x.IsQuotedStatus == null ? x.Text : string.Concat(x.Text, x.QuotedStatus.Text), Id = x.Id, Place = x.Place?.Name, Language = x.Language, RetweetCount = x.RetweetCount, FavoriteCount = x.FavoriteCount, User = x.User.ScreenName, QuotedTweet = x.QuotedStatus?.Text });
            return model;
        }

        public async Task<SearchResult> Search(string searchTerm)
        {
            return await this.tokens.Search.TweetsAsync(q: searchTerm, lang: "en", count: 5);
        }

        public async Task<StatusResponse> SendTweet(SendTweetModel status)
        {
            return await this.tokens.Statuses.UpdateAsync(status.Message);
        }

        public async Task<StatusResponse> DeleteTweet(long id)
        {
            try
            {
                return await this.tokens.Statuses.DestroyAsync(id);
            }
            catch (TwitterException ex)
            {
                throw new InvalidOperationException("Cannot delete tweet", ex);
            }
        }

        public async Task<Tweet> GetTweet(long id)
        {
            var data = await this.tokens.Statuses.ShowAsync(id);
            return new Tweet() { CreatedAt = data.CreatedAt, Body = data.Text, QuotedTweet = data.QuotedStatus?.Text, Id = data.Id, Place = data.Place?.Name, Language = data.Language, RetweetCount = data.RetweetCount, FavoriteCount = data.FavoriteCount, User = data.User.ScreenName };
        }
    }
}