using AccountService.Common.Enums;
using AccountService.Features.Transactions.Models;

namespace AccountService.Features.Transactions;

public class TransactionDto
{
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
    public decimal Amount { get; init; }
    
    /// <summary>
    /// Валюта.
    /// </summary>
    public IsoCurrency Currency { get; init; }
    
    /// <summary>
    /// Тип транзакции.
    /// </summary>
    public TransactionType Type { get; set; }
    
    /// <summary>
    /// Описание.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}