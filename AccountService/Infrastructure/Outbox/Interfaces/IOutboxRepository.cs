using AccountService.Infrastructure.Dapper.Interfaces;

namespace AccountService.Infrastructure.Outbox.Interfaces;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, ITransaction? transaction = null);
    Task<IReadOnlyList<OutboxMessage>> TakeDueAsync(int batchSize);
    Task MarkPublishedAsync(Guid id);
    Task MarkFailedAsync(Guid id, DateTime nextAttemptAt);
    Task<int> GetPendingCountAsync();
}