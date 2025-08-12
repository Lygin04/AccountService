using AccountService.Features.Accounts;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Dapper.Models;

namespace AccountService.Infrastructure;

public class AccrueInterestHandler(IDapperContext<IDapperSettings> context, IAccountRepository accountRepository)
{
    public async Task HandleAsync()
    {
        using var transaction = context.BeginTransaction();
        try
        {
            var accountIds = await accountRepository.GetAllAccountIdsAsync();

            foreach (var id in accountIds)
            {
                await transaction.Command(
                    new QueryObject("CALL accrue_interest(@Id)", new { Id = id }));
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}