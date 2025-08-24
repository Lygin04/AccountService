// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json.Serialization;

namespace AccountService.Features.Accounts.UpdateAccount;

public class UpdateAccountResponseDto
{
    /// <summary>
    /// Баланс.
    /// </summary>
    public decimal Balance { get; init; }

    /// <summary>
    /// Процентная ставка (decimal, опционально — только для Deposit и Credit).
    /// </summary>
    public decimal? InterestRate { get; init; }
    
    [JsonIgnore]
    public int Xmin { get; set; }
}