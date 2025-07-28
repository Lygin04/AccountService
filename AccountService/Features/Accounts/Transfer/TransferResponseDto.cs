using AccountService.Common.Enums;
using AccountService.Features.Transactions.Models;

namespace AccountService.Features.Accounts.Transfer;

public class TransferResponseDto
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
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Валюта.
    /// </summary>
    public IsoCurrency Currency { get; set; }
    
    /// <summary>
    /// Тип (Credit | Debit).
    /// </summary>
    public TransactionType Type { get; set; }
}