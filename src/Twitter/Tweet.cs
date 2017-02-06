using System;

namespace TwitterMW
{
    public class Tweet
    {
        public DateTimeOffset CreatedAt { get; set; }

        public string Body { get; set; }

        public string QuotedTweet { get; set; }

        public long Id { get; set; }

        public string Place { get; set; }

        public string Language { get; set; }

        public int? RetweetCount { get; set; }

        public int? FavoriteCount { get; set; }

        public string User { get; set; }
    }
}