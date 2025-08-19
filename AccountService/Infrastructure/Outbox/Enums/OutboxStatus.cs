namespace AccountService.Infrastructure.Outbox.Enums;

public enum OutboxStatus
{
    Pending,
    // ReSharper disable once UnusedMember.Global
    Published,
    // ReSharper disable once UnusedMember.Global
    Failed,
    // ReSharper disable once UnusedMember.Global
    Dead
}