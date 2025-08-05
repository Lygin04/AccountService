using AccountService.Common.Enums;
using AccountService.Features.Accounts.Models;

namespace AccountService.Features.Accounts.CreateAccount;

public class CreateAccountResponseDto
{
    /// <summary>
    /// Идентификатор владельца счета.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global сеттер нужен для задавания значния через клиента
    public Guid OwnerId { get; set; }
    
    /// <summary>
    /// Тип (Checking | Deposit | Credit).
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global сеттер нужен для задавания значния через клиента
    public AccountType Type { get; set; }
    
    /// <summary>
    /// Валюта (ISO 4217).
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global сеттер нужен для задавания значния через клиента
    public IsoCurrency Currency { get; set; }
    
    /// <summary>
    /// Баланс.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global сеттер нужен для задавания значния через клиента
    public decimal Balance { get; set; }

    /// <summary>
    /// Процентная ставка (decimal, опционально — только для Deposit и Credit).
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global сеттер нужен для задавания значния через клиента
    public decimal? InterestRate { get; set; }
}