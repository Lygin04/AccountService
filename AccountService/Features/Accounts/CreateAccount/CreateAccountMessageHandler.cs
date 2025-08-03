using AccountService.Common.Abstractions;
using AccountService.Exceptions.Account;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Clients.Interfaces;
using AccountService.Infrastructure.Repositories.Interfaces;
using MediatR;

namespace AccountService.Features.Accounts.CreateAccount;

public class CreateAccountMessageHandler(
    IFakeDataStorage fakeDataStorage,
    IClientVerificationService clientVerification,
    ICurrencyService currencyService) : IMessageHandler<CreateAccountMessage, Unit>
{
    public async Task<Unit> Handle(CreateAccountMessage request, CancellationToken cancellationToken)
    {
        if (!clientVerification.ClientExists(request.CreateAccountResponseDto.OwnerId))
            throw AccountNotFoundException.WithSuchOwnerId(request.CreateAccountResponseDto.OwnerId);

        if (!currencyService.IsSupported(request.CreateAccountResponseDto.Currency.ToString()))
            throw AccountBadRequestException.UnsupportedCurrencyException(request.CreateAccountResponseDto.Currency
                .ToString());

        var account = new Account
        {
            Id = Guid.NewGuid(),
            OwnerId = request.CreateAccountResponseDto.OwnerId,
            Type = request.CreateAccountResponseDto.Type,
            Currency = request.CreateAccountResponseDto.Currency,
            Balance = request.CreateAccountResponseDto.Balance,
            InterestRate = request.CreateAccountResponseDto.InterestRate,
            OpenDate = DateTime.Now
        };

        await fakeDataStorage.AddAccountAsync(account);
        return Unit.Value;
    }
}