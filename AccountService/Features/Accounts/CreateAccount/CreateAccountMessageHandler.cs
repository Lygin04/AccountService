using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Clients.Interfaces;
using AccountService.Infrastructure.Repositories.Interfaces;
using FluentValidation;
using MediatR;

namespace AccountService.Features.Accounts.CreateAccount;

public class CreateAccountMessageHandler(
    IFakeDataStorage fakeDataStorage,
    IClientVerificationService clientVerification,
    ICurrencyService currencyService,
    IValidator<CreateAccountMessage> validator) : IMessageHandler<CreateAccountMessage, MbResult<Unit>>
{
    public async Task<MbResult<Unit>> Handle(CreateAccountMessage request, CancellationToken cancellationToken)
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

        if (!clientVerification.ClientExists(request.CreateAccountResponseDto.OwnerId))
        {
            return MbResult<Unit>.Failure(new MbError(
                title: "Not Found",
                status: StatusCodes.Status404NotFound,
                detail: $"Client with OwnerId {request.CreateAccountResponseDto.OwnerId} not found"
            ));
        }

        if (!currencyService.IsSupported(request.CreateAccountResponseDto.Currency.ToString()))
        {
            return MbResult<Unit>.Failure(new MbError(
                title: "Bad Request",
                status: StatusCodes.Status400BadRequest,
                detail: $"Currency {request.CreateAccountResponseDto.Currency} is not supported"
            ));
        }

        var account = new Account
        {
            Id = Guid.NewGuid(),
            OwnerId = request.CreateAccountResponseDto.OwnerId,
            Type = request.CreateAccountResponseDto.Type,
            Currency = request.CreateAccountResponseDto.Currency,
            Balance = request.CreateAccountResponseDto.Balance,
            InterestRate = request.CreateAccountResponseDto.InterestRate,
            OpenDate = DateTime.UtcNow
        };

        await fakeDataStorage.AddAccountAsync(account);
        return MbResult<Unit>.Success(Unit.Value);
    }
}