using FluentValidation;

namespace TwitterMW
{
    public class TweetValidator : AbstractValidator<SendTweetModel>
    {
        public TweetValidator()
        {
            this.RuleFor(x => x.Message).NotEmpty();
        }
    }
}