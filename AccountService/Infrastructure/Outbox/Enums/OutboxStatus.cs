namespace AccountService.Infrastructure.Outbox.Enums;

public enum OutboxStatus
{
    Pending,
    Published,
    Failed,
    Dead
}