using AccountService.Common.Enums;
using AccountService.Features.Accounts.Models;

namespace AccountService.Features.Accounts.CreateAccount;

public class CreateAccountResponseDto
{
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
}