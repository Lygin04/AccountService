using FluentValidation;

namespace AccountService.Features.Accounts.Transfer;

// ReSharper disable once UnusedType.Global
public class TransferMessageValidator: AbstractValidator<TransferMessage>
{
    public TransferMessageValidator()
    {
        RuleFor(x => x.TransferDto.AccountId)
            .NotEmpty();
        
        RuleFor(x => x.TransferDto.CounterpartyAccountId)
            .NotEmpty();
        
        RuleFor(x => x.TransferDto.Amount)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.TransferDto.Currency)
            .IsInEnum();
        
        RuleFor(x => x.TransferDto.Type)
            .IsInEnum();
    }
}