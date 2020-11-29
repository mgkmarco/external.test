using External.Test.Host.Contracts.Public.Models;
using FluentValidation;

namespace External.Test.Host.Validators
{
    public class MarketSelectionRequestValidator : AbstractValidator<MarketSelectionUpdateRequest>
    {
        public MarketSelectionRequestValidator()
        {
            RuleFor(s => s.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(255)
                .WithErrorCode("INVALID_MARKET_SELECTION_NAME");
            
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(1)
                .WithErrorCode("INVALID_MARKET_SELECTION_PRICE");
        }
            
    }
}