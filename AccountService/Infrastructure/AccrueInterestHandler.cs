using AccountService.Contracts;
using AccountService.Features.Accounts;
using AccountService.Features.Accounts.Models;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Dapper.Models;
using AccountService.Infrastructure.RabbitMq.Interfaces;

namespace AccountService.Infrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
public class AccrueInterestHandler(
    IDapperContext<IDapperSettings> dapperContext,
    IAccountRepository accountRepository,
    IEventPublisher eventPublisher)
{
    public async Task HandleAsync()
    {
        using var transaction = dapperContext.BeginTransaction();
        try
        {
            var accountIds = await accountRepository.GetAllAccountIdsAsync();

            foreach (var id in accountIds)
            {
                var account = await accountRepository.GetByIdAsync(id);
                if(account?.Type is AccountType.Checking )
                    continue;
                var dailyInterest = account?.Balance * (account?.InterestRate / 100.00m / 365.00m);
                await transaction.Command(
                    new QueryObject("CALL accrue_interest(@Id)", new { Id = id }));

                await eventPublisher.PublishAsync(new InterestAccrued(
                    Guid.NewGuid(), 
                    DateTime.UtcNow, 
                    id,
                    DateTime.UtcNow.Date,
                    DateTime.UtcNow.Date.AddDays(-1),
                    dailyInterest),
                    routingKey: "account.notifications",
                    correlationId: Guid.NewGuid(),
                    causationId: Guid.NewGuid());       // TODO: Пересмотреть данное решение
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