using AccountService.Exceptions.Shared;

namespace AccountService.Exceptions.Account;

// ReSharper disable once UnusedType.Global
public class AccountBadRequestException(string message) : BadRequestException(message)
{
    // ReSharper disable once UnusedMember.Global
    public static AccountBadRequestException UnsupportedCurrencyException(string currencyCode)
    {
        return new AccountBadRequestException($"Currency '{currencyCode}' is not supported.");
    }
    
    // ReSharper disable once UnusedMember.Global
    public static AccountBadRequestException InsufficientFundsException(Guid accountId, decimal balance, decimal attemptedAmount)
    {
        return new AccountBadRequestException(
            $"Account '{accountId}' has insufficient funds. Current balance: {balance}, attempted withdrawal: {attemptedAmount}."
        );
    }
}