using AccountService.Common.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace AccountService.Contracts.Examples;

// ReSharper disable once UnusedType.Global
public class TransferCompletedExample : IExamplesProvider<TransferCompleted>
{
    public TransferCompleted GetExamples()
    {
        return new TransferCompleted(
            EventId: Guid.NewGuid(),
            OccurredAt: DateTime.UtcNow,
            SourceAccountId: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            DestinationAccountId: Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Amount: 1500.75m,
            Currency: IsoCurrency.RUB,
            TransferId: Guid.NewGuid()
        );
    }
}