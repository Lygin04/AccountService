using AccountService.Common.Enums;

namespace AccountService.Features.Transactions.Models;

public class DbTransaction
{
    /// <summary>
    /// Идентификатор транзакции.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Guid Id { get; init; }
    
    /// <summary>
    /// Идентификатор счета.
    /// </summary>
    public Guid AccountId { get; init; }

    /// <summary>
    /// Идентификатор счета контрагента.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Guid CounterpartyAccountId { get; set; }
    
    /// <summary>
    /// Сумма.
    /// </summary>
    public decimal Amount { get; init; }
    
    /// <summary>
    /// Валюта.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public IsoCurrency Currency { get; init; }
    
    /// <summary>
    /// Тип (Credit | Debit).
    /// </summary>
    public TransactionType Type { get; init; }
    
    /// <summary>
    /// Описание.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата/Время.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime Timestamp { get; set; }
}