using FluentValidation;

namespace AccountService.Features.Accounts.UpdateAccount;

public sealed class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.AccountDto.Balance)
            .NotEmpty();
        
    }
}