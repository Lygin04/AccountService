using FluentValidation;

namespace AccountService.Features.Accounts.UpdateAccount;

public sealed class UpdateAccountMessageValidator : AbstractValidator<UpdateAccountMessage>
{
    public UpdateAccountMessageValidator()
    {
        RuleFor(x => x.AccountDto.Balance)
            .NotEmpty();
        
    }
}