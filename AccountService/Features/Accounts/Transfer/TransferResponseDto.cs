using AccountService.Common.Enums;
using AccountService.Features.Transactions.Models;

namespace AccountService.Features.Accounts.Transfer;

public class TransferResponseDto
{
    /// <summary>
    /// Идентификатор счета.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Guid AccountId { get; init; }

    /// <summary>
    /// Идентификатор счета контрагента.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Guid CounterpartyAccountId { get; init; }
    
    /// <summary>
    /// Сумма.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public decimal Amount { get; init; }
    
    /// <summary>
    /// Валюта.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public IsoCurrency Currency { get; init; }
    
    /// <summary>
    /// Тип (Credit | Debit).
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public TransactionType Type { get; init; }
}