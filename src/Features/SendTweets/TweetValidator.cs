using FluentValidation;

namespace TwitterMW
{
    public class TweetValidator : AbstractValidator<Tweet>
    {
        public TweetValidator()
        {
            this.RuleFor(x => x.Message).NotEmpty();
        }
    }
}