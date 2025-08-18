using System.Net.Http.Json;
using AccountService.Common.Enums;
using AccountService.Features.Accounts.CreateAccount;
using AccountService.Features.Accounts.Models;
using AccountService.Features.Accounts.Transfer;
using AccountService.Features.Transactions.Models;

namespace AccountService.Tests;

[Collection("IntegrationTests")]
public class ParallelTransferTests(CustomWebApplicationFactory<ProgramPlaceholder> factory)
    : IClassFixture<CustomWebApplicationFactory<ProgramPlaceholder>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ParallelTransfers_ShouldMaintainTotalBalance()
    {
        // Arrange
        
        var account1 = new CreateAccountResponseDto
        {
            OwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Balance = 10000,
            InterestRate = 0,
            Currency = IsoCurrency.RUB,
            Type = AccountType.Checking
        };
        
        var account2 = new CreateAccountResponseDto
        {
            OwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Balance = 0,
            InterestRate = 0,
            Currency = IsoCurrency.RUB,
            Type = AccountType.Checking
        };

        await _client.PostAsJsonAsync("/v1/Accounts", account1);
        await _client.PostAsJsonAsync("/v1/Accounts", account2);
        
        var response = await _client.GetAsync("v1/Accounts/owner/11111111-1111-1111-1111-111111111111");
        
        var accounts = await response.Content.ReadFromJsonAsync<List<DbAccount>>();

        if (accounts != null)
        {
            var fromAccountId = accounts[0].Id;
            var toAccountId = accounts[1].Id;

            // Предположим, что в БД есть два счета с начальными балансами:
            // fromAccount: 10000, toAccount: 0

            const int transferCount = 50;
            const decimal transferAmount = 100m;

            var tasks = new List<Task<HttpResponseMessage>>();

            for (var i = 0; i < transferCount; i++)
            {
                var transferDto = new TransferResponseDto
                {
                    AccountId = fromAccountId,
                    CounterpartyAccountId = toAccountId,
                    Amount = transferAmount,
                    Currency = IsoCurrency.USD,
                    Type = TransactionType.Debit
                };

                tasks.Add(_client.PostAsJsonAsync("v1/Accounts/transfer", transferDto));
            }

            // Act
            await Task.WhenAll(tasks);

            // Assert
            var fromAccountResp = await _client.GetAsync($"v1/Accounts/{fromAccountId.ToString()}");
            var fromAccount = await fromAccountResp.Content.ReadFromJsonAsync<AccountDto>();

            var toAccountResp = await _client.GetAsync($"v1/Accounts/{toAccountId.ToString()}");
            var toAccount = await toAccountResp.Content.ReadFromJsonAsync<AccountDto>();

            if (fromAccount != null && toAccount != null)
            {
                var totalBalance = fromAccount.Balance + toAccount.Balance;
                const decimal expectedTotalBalance = 10000m;
                Assert.Equal(expectedTotalBalance, totalBalance);
            }
        }
    }

    private record AccountDto(Guid Id, decimal Balance);
}