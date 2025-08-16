using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Dapper.Models;
using AccountService.Infrastructure.Outbox.Interfaces;

namespace AccountService.Infrastructure.Outbox;

public class OutboxRepository(IDapperContext<IDapperSettings> dapperContext) : IOutboxRepository
{
    public async Task AddAsync(OutboxMessage message, ITransaction? transaction = null)
    {
        await dapperContext.Command(new QueryObject(Scripts.Outbox.Outbox.Create, message), transaction);
    }

    public async Task<IReadOnlyList<OutboxMessage>> TakeDueAsync(int batchSize)
    {
        return await dapperContext.ListOrEmpty<OutboxMessage>(new QueryObject(Scripts.Outbox.Outbox.TakeDue,
            new { batch = batchSize }));
    }

    public async Task MarkPublishedAsync(Guid id)
    {
        await dapperContext.Command(new QueryObject(Scripts.Outbox.Outbox.MarkPublished, new { Id = id }));
    }

    public async Task MarkFailedAsync(Guid id, DateTime nextAttemptAt)
    {
        await dapperContext.Command(new QueryObject(Scripts.Outbox.Outbox.MarkFailed,
            new { Id = id, NextAttemptAt = nextAttemptAt }));
    }

    public async Task<int> GetPendingCountAsync()
    {
        return await dapperContext.FirstOrDefault<int>(new QueryObject(Scripts.Outbox.Outbox.GetPendingCount));
    }
}