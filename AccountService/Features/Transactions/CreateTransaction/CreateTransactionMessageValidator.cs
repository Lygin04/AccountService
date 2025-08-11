using FluentValidation;

namespace AccountService.Features.Transactions.CreateTransaction;

// ReSharper disable once UnusedType.Global
public class CreateTransactionMessageValidator : AbstractValidator<CreateTransactionMessage>
{
    public CreateTransactionMessageValidator()
    {
        RuleFor(x => x.TransactionDto.AccountId)
            .NotEmpty()
            .NotEqual(Guid.Empty);
        
        RuleFor(x => x.TransactionDto.CounterpartyAccountId)
            .NotEmpty()
            .NotEqual(Guid.Empty);
        
        RuleFor(x => x.TransactionDto.Amount)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.TransactionDto.Currency)
            .NotEmpty()
            .IsInEnum();
        
        RuleFor(x => x.TransactionDto.Type)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.TransactionDto.Description)
            .NotEmpty();
    }
}