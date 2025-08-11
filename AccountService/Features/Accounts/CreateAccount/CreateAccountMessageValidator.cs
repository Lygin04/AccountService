using AccountService.Features.Accounts.Models;
using FluentValidation;

namespace AccountService.Features.Accounts.CreateAccount;

// ReSharper disable once UnusedType.Global
public sealed class CreateAccountMessageValidator : AbstractValidator<CreateAccountMessage>
{
    public CreateAccountMessageValidator()
    {
        RuleFor(x => x.CreateAccountResponseDto.OwnerId)
            .NotNull()
            .NotEqual(Guid.Empty);
        
        RuleFor(x => x.CreateAccountResponseDto.Type)
            .NotNull()
            .IsInEnum();
        
        RuleFor(x => x.CreateAccountResponseDto.Currency)
            .NotNull()
            .IsInEnum();

        RuleFor(x => x.CreateAccountResponseDto.Balance)
            .NotNull()
            .GreaterThanOrEqualTo(0m);

        RuleFor(x => x.CreateAccountResponseDto.InterestRate)
            .NotNull()
            .InclusiveBetween(0.01m, 100.00m)
            .When(x => x.CreateAccountResponseDto.Type is AccountType.Deposit or AccountType.Credit);
    }
}