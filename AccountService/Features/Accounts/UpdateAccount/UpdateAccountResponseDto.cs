namespace AccountService.Features.Accounts.UpdateAccount;

public class UpdateAccountResponseDto
{
    /// <summary>
    /// Баланс.
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Процентная ставка (decimal, опционально — только для Deposit и Credit).
    /// </summary>
    public decimal? InterestRate { get; set; }
}