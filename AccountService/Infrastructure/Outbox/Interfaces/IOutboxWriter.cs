using AccountService.Contracts;
using AccountService.Infrastructure.Dapper.Interfaces;

namespace AccountService.Infrastructure.Outbox.Interfaces;

public interface IOutboxWriter
{
    Task WriteAsync<TPayload>(
        string routingKey,
        Envelope<TPayload> envelope,
        ITransaction? transaction = null);
}