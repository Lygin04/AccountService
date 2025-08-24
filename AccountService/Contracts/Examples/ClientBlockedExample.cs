using AccountService.Common.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace AccountService.Contracts.Examples;

// ReSharper disable once UnusedType.Global
public class ClientBlockedExample : IExamplesProvider<ClientBlocked>
{
    public ClientBlocked GetExamples() => new(
        EventId: Guid.NewGuid(),
        OccurredAt: DateTime.UtcNow,
        ClientId: Guid.NewGuid()
    );
}

// ReSharper disable once UnusedType.Global
public class ClientUnblockedExample : IExamplesProvider<ClientUnblocked>
{
    public ClientUnblocked GetExamples() => new(
        EventId: Guid.NewGuid(),
        OccurredAt: DateTime.UtcNow,
        ClientId: Guid.NewGuid()
    );
}

// ReSharper disable once UnusedType.Global
public class InterestAccruedExample : IExamplesProvider<InterestAccrued>
{
    public InterestAccrued GetExamples()
    {
        return new InterestAccrued(
            EventId: Guid.NewGuid(),
            OccurredAt: DateTime.UtcNow,
            AccountId: Guid.NewGuid(),
            PeriodFrom: DateTime.UtcNow.AddMonths(-1),
            PeriodTo: DateTime.UtcNow,
            Amount: 1000
        );
    }
}

// ReSharper disable once UnusedType.Global
public class MoneyCreditedExample : IExamplesProvider<MoneyCredited>
{
    public MoneyCredited GetExamples()
    {
        return new MoneyCredited(
            EventId: Guid.NewGuid(),
            OccurredAt: DateTime.UtcNow,
            AccountId: Guid.NewGuid(),
            Amount: 1000,
            Currency: IsoCurrency.RUB,
            OperationId: Guid.NewGuid()
        );
    }
}

// ReSharper disable once UnusedType.Global
public class MoneyDebitedExample : IExamplesProvider<MoneyDebited>
{
    public MoneyDebited GetExamples()
    {
        return new MoneyDebited(
            EventId: Guid.NewGuid(),
            OccurredAt: DateTime.UtcNow,
            AccountId: Guid.NewGuid(),
            Amount: 1000,
            Currency: IsoCurrency.RUB,
            OperationId: Guid.NewGuid(),
            Reason: "Payment"
        );
    }
}