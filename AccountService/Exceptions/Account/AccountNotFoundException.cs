using AccountService.Exceptions.Shared;

namespace AccountService.Exceptions.Account;

// ReSharper disable once UnusedType.Global
public class AccountNotFoundException(string message) : NotFoundException(message)
{
    // ReSharper disable once UnusedMember.Global
    public static AccountNotFoundException WithSuchOwnerId(Guid ownerId)
    {
        return new AccountNotFoundException($"Owner Id '{ownerId}' has not been found.");
    }
    
    // ReSharper disable once UnusedMember.Global
    public static AccountNotFoundException WithSuchId(Guid id)
    {
        return new AccountNotFoundException($"Account with id '{id}' has not been found");
    }
}