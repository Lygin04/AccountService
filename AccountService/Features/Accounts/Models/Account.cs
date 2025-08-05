using AccountService.Common.Enums;
using AccountService.Features.Transactions.Models;

namespace AccountService.Features.Accounts.Models;

public class Account
{
    /// <summary>
    /// Идентификатор счета.
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Идентификатор владельца счета.
    /// </summary>
    public Guid OwnerId { get; set; }
    
    /// <summary>
    /// Тип (Checking | Deposit | Credit).
    /// </summary>
    public AccountType Type { get; set; }
    
    /// <summary>
    /// Валюта (ISO 4217).
    /// </summary>
    public IsoCurrency Currency { get; set; }
    
    /// <summary>
    /// Баланс.
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Процентная ставка (decimal, опционально — только для Deposit и Credit).
    /// </summary>
    public decimal? InterestRate { get; set; }
    
    /// <summary>
    /// Дата открытия.
    /// </summary>
    public DateTime OpenDate { get; set; }
    
    /// <summary>
    /// Дата закрытия (опционально).
    /// </summary>
    public DateTime? CloseDate { get; set; }
    
    /// <summary>
    /// Коллекция транзакций.
    /// </summary>
    public List<Transaction>? Transactions { get; set; }
}