using AccountService.Common.Enums;

namespace AccountService.Features.Transactions.Models;

public class Transaction
{
    /// <summary>
    /// Идентификатор транзакции.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Идентификатор счета.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Идентификатор счета контрагента.
    /// </summary>
    public Guid CounterpartyAccountId { get; set; }
    
    /// <summary>
    /// Сумма.
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Валюта.
    /// </summary>
    public IsoCurrency Currency { get; set; }
    
    /// <summary>
    /// Тип (Credit | Debit).
    /// </summary>
    public TransactionType Type { get; set; }
    
    /// <summary>
    /// Описание.
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Дата/Время.
    /// </summary>
    public DateTime Timestamp { get; set; }
}