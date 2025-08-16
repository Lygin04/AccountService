using AccountService.Contracts;

namespace AccountService.Infrastructure.Outbox;

public static class EnvelopeFactory
{
    public static Envelope<T> Create<T>(T payload,
        Guid eventId,
        DateTime occurredAtUtc,
        Guid correlationId,
        Guid causationId,
        string source = "account-service")
        => new(eventId, occurredAtUtc, new Meta("v1", source, correlationId, causationId), payload);    // TODO: Удалить Meta
}