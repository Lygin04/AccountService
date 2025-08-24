using AccountService.Contracts;
using AccountService.Infrastructure.Dapper.Interfaces;

namespace AccountService.Infrastructure.Outbox.Interfaces;

public interface IOutboxWriter
{
    Task WriteAsync<TPayload>(
        string routingKey,
        TPayload envelope,
        ITransaction? transaction = null)
        where TPayload : Event;
}