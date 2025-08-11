using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Infrastructure.Repositories.Interfaces;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Accounts.UpdateAccount;

public class UpdateAccountMessageHandler(
    IFakeDataStorage fakeDataStorage,
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
        
        var account = await fakeDataStorage.GetAccountByIdAsync(request.AccountId);
        
        if (account == null)
        {
            return MbResult<Unit>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Account with ID {request.AccountId} not found"
            ));
        }
        
        account.Balance = request.AccountDto.Balance;
        account.InterestRate = request.AccountDto.InterestRate;
        
        await fakeDataStorage.UpdateAccountAsync(account);
        return MbResult<Unit>.Success(Unit.Value);
    }
}