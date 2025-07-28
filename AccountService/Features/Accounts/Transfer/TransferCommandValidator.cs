using FluentValidation;

namespace AccountService.Features.Accounts.Transfer;

public class TransferCommandValidator: AbstractValidator<TransferCommand>
{
    public TransferCommandValidator()
    {
        RuleFor(x => x.TransferDto.AccountId)
            .NotEmpty();
        
        RuleFor(x => x.TransferDto.CounterpartyAccountId)
            .NotEmpty();
        
        RuleFor(x => x.TransferDto.Amount)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.TransferDto.Currency)
            .NotEmpty()
            .IsInEnum();
        
        RuleFor(x => x.TransferDto.Type)
            .NotEmpty()
            .IsInEnum();
    }
}