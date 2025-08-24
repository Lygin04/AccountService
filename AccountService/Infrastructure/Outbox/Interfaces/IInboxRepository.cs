namespace AccountService.Infrastructure.Outbox.Interfaces;

public interface IInboxRepository
{
    Task<bool> ExistsAsync(Guid messageId, string handler);
    Task AddConsumedAsync(Guid messageId, string handler);
    Task AddDeadLetterAsync(Guid messageId, string handler, string payloadJson, string error);
}