using AccountService.Exceptions.Shared;

namespace AccountService.Features.Accounts;

public class AccountBadRequestException(string message) : BadRequestException(message)
{
    public static AccountBadRequestException UnsupportedCurrencyException(string currencyCode)
    {
        return new AccountBadRequestException($"Currency '{currencyCode}' is not supported.");
    }
    
    public static AccountBadRequestException InsufficientFundsException(Guid accountId, decimal balance, decimal attemptedAmount)
    {
        return new AccountBadRequestException(
            $"Account '{accountId}' has insufficient funds. Current balance: {balance}, attempted withdrawal: {attemptedAmount}."
        );
    }
}