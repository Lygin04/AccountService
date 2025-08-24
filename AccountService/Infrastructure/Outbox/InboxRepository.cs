using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Dapper.Models;
using AccountService.Infrastructure.Outbox.Interfaces;
using AccountService.Infrastructure.Scripts.Inbox;

namespace AccountService.Infrastructure.Outbox;

public class InboxRepository(IDapperContext<IDapperSettings> dapperContext) : IInboxRepository
{
    public async Task<bool> ExistsAsync(Guid messageId, string handler)
    {
        return await dapperContext.CommandWithResponse<bool>(new QueryObject(Inbox.Exists,
            new { MessageId = messageId, Handler = handler }));
    }

    public async Task AddConsumedAsync(Guid messageId, string handler)
    {
        await dapperContext.Command(
            new QueryObject(Inbox.AddConsumed, new { MessageId = messageId, Handler = handler }));
    }

    public async Task AddDeadLetterAsync(Guid messageId, string handler, string payloadJson, string error)
    {
        await dapperContext.Command(new QueryObject(Inbox.AddDeadLetter,
            new { MessageId = messageId, Handler = handler, PayloadJson = payloadJson, Error = error }));
    }
}