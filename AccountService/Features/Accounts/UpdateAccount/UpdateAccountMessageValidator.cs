using FluentValidation;

namespace AccountService.Features.Accounts.UpdateAccount;

// ReSharper disable once UnusedType.Global
public sealed class UpdateAccountMessageValidator : AbstractValidator<UpdateAccountMessage>
{
    public UpdateAccountMessageValidator()
    {
        RuleFor(x => x.AccountDto.Balance)
            .NotEmpty();
    }
}