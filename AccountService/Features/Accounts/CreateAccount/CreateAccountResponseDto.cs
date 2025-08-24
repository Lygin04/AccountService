using AccountService.Common.Enums;
using AccountService.Features.Accounts.Models;

namespace AccountService.Features.Accounts.CreateAccount;

public class CreateAccountResponseDto
{
    /// <summary>
    /// Идентификатор владельца счета.
    /// </summary>
    public Guid OwnerId { get; init; }
    
    /// <summary>
    /// Тип (Checking | Deposit | Credit).
    /// </summary>
    public AccountType Type { get; init; }
    
    /// <summary>
    /// Валюта (ISO 4217).
    /// </summary>
    public IsoCurrency Currency { get; init; }
    
    /// <summary>
    /// Баланс (для тестов).
    /// </summary>
    public decimal Balance { get; init; }

    /// <summary>
    /// Процентная ставка (decimal, опционально — только для Deposit и Credit).
    /// </summary>
    public decimal? InterestRate { get; init; }
}