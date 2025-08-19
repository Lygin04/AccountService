using AccountService.Common;
using AccountService.Common.Abstractions;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Accounts.UpdateAccount;

public class UpdateAccountMessageHandler(
    IAccountRepository accountRepository,
    IValidator<UpdateAccountMessage> validator) : IMessageHandler<UpdateAccountMessage, MbResult<Unit>>
{
    public async Task<MbResult<Unit>> Handle(UpdateAccountMessage request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            
            return MbResult<Unit>.Failure(new MbError(
                title: "Validation Error",
                status: StatusCodes.Status422UnprocessableEntity,
                detail: "Validation failed",
                errors: errors
            ));
        }
        
        var account = await accountRepository.GetByIdAsync(request.AccountId);
        
        if (account == null)
        {
            return MbResult<Unit>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Account with ID {request.AccountId} not found"
            ));
        }

        var updateAccountResponseDto = new UpdateAccountResponseDto
        {
            Balance = request.AccountDto.Balance,
            InterestRate = request.AccountDto.InterestRate
        };
        
        account.Balance = request.AccountDto.Balance;
        account.InterestRate = request.AccountDto.InterestRate;
        
        await accountRepository.UpdateAsync(account.Id, updateAccountResponseDto);
        return MbResult<Unit>.Success(Unit.Value);
    }
}