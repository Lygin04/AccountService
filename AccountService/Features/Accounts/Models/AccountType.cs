using System.Text.Json.Serialization;

namespace AccountService.Features.Accounts.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountType
{
    Checking,
    Deposit,
    Credit
}