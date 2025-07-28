using AccountService.Exceptions.Shared;

namespace AccountService.Features.Accounts;

public class AccountNotFoundException(string message) : NotFoundException(message)
{
    public static AccountNotFoundException WithSuchOwnerId(Guid ownerId)
    {
        return new AccountNotFoundException($"Owner Id '{ownerId}' has not been found.");
    }
    
    public static AccountNotFoundException WithSuchId(Guid id)
    {
        return new AccountNotFoundException($"Account with id '{id}' has not been found");
    }
}