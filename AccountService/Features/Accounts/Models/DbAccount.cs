using System.Text.Json.Serialization;
using AccountService.Common.Enums;
using AccountService.Features.Transactions.Models;

namespace AccountService.Features.Accounts.Models;

public class DbAccount
{
    /// <summary>
    /// Идентификатор счета.
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Идентификатор владельца счета.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Guid OwnerId { get; init; }
    
    /// <summary>
    /// Тип (Checking | Deposit | Credit).
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public AccountType Type { get; init; }
    
    /// <summary>
    /// Валюта (ISO 4217).
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public IsoCurrency Currency { get; init; }
    
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
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime OpenDate { get; set; }
    
    /// <summary>
    /// Дата закрытия (опционально).
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public DateTime? CloseDate { get; set; }
    
    // ReSharper disable once UnusedMember.Global
    [JsonIgnore]
    public int Xmin { get; set; }
    
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    /// <summary>
    /// Флаг блокировки счета.
    /// </summary>
    public bool IsBlocked { get; set; } = false;
    
    /// <summary>
    /// Коллекция транзакций.
    /// </summary>
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<DbTransaction>? Transactions { get; set; }
}