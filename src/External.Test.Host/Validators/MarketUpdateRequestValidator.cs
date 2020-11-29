using External.Test.Host.Contracts.Public.Models;
using FluentValidation;

namespace External.Test.Host.Validators
{
    public class MarketUpdateRequestValidator : AbstractValidator<MarketUpdateRequest>
    {
        public MarketUpdateRequestValidator()
        {
            RuleFor(x => x.MarketId)
                .GreaterThanOrEqualTo(1)
                .WithErrorCode("INVALID_MARKET_ID");
            
            RuleFor(x => x.MarketType)
                .NotNull()
                .NotEmpty()
                .MaximumLength(255)
                .WithErrorCode("INVALID_MARKET_TYPE");
            
            RuleFor(x => x.MarketState)
                .IsInEnum()
                .WithErrorCode("INVALID_MARKET_STATE");

            RuleForEach(x => x.Selections)
                .NotNull()
                .NotEmpty()
                .SetValidator(new MarketSelectionRequestValidator());
        }
    }
}