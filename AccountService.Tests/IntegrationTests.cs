using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using AccountService.Common.Enums;
using AccountService.Features.Accounts.CreateAccount;
using AccountService.Features.Accounts.Models;
using AccountService.Features.Accounts.Transfer;
using AccountService.Features.Transactions.Models;
using AccountService.Infrastructure.Outbox;
using AccountService.Infrastructure.Outbox.Enums;
using AccountService.Infrastructure.Outbox.Interfaces;
using AccountService.Infrastructure.RabbitMq;
using Microsoft.Extensions.Logging;

namespace AccountService.Tests;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<AppTestContainers> { }

[Collection("IntegrationTests")]
public class IntegrationTests
{
    private readonly HttpClient _client;
    
    private readonly RabbitMqConnection _rabbitConn;
    private readonly IOutboxRepository _outboxRepository;
    private OutboxDispatcher _outboxDispatcher;
    private readonly ILogger<OutboxDispatcher> _logger;

    
    public IntegrationTests(AppTestContainers containers)
    {
        var factory = new CustomWebApplicationFactory<ProgramPlaceholder>(
            containers.PostgresConnectionString,
            containers.RabbitHost,
            containers.RabbitPort);

        _rabbitConn = containers.RabbitConn;
        _outboxRepository = containers.OutboxRepository;
        _logger = containers.Logger;
        _outboxDispatcher = containers.OutboxDispatcher;
        
        _client = factory.CreateClient();
    }
    
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
                    Currency = IsoCurrency.RUB,
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

    [Fact]
    public async Task OutboxPublishesAfterFailure()
    {
        // Arrange
        var msg = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            Type = "AccountCreated",
            RoutingKey = "account.created",
            PayloadJson = JsonSerializer.Serialize(new { AccountId = Guid.NewGuid() }),
            HeadersJson = "{}",
            Attempts = 0,
            Status = OutboxStatus.Pending
        };
        await _outboxRepository.AddAsync(msg);

        // 1. Имитация отключения RabbitMQ
        _rabbitConn.Dispose();

        // Act 1 — публикация не удалась
        await _outboxDispatcher.DispatchBatch();

        var maxWait = TimeSpan.FromSeconds(5);
        var sw = Stopwatch.StartNew();
        OutboxMessage? failed = null;

        while (sw.Elapsed < maxWait)
        {
            failed = (await _outboxRepository.TakeDueAsync(10))
                .FirstOrDefault(m => m.Id == msg.Id);
            if (failed != null) break;
            await Task.Delay(50);
        }

        // Assert 1
        Assert.NotNull(failed);
        Assert.True(failed.Attempts > 0);

        // 2. Заново подключаемся к RabbitMQ
        _rabbitConn.Reconnect();

        _outboxDispatcher = new OutboxDispatcher(_outboxRepository, _rabbitConn, _logger);
        
        await _outboxDispatcher.DispatchBatch();

        var pending = await _outboxRepository.GetPendingCountAsync();

        Assert.Equal(0, pending);
    }
    
    [Fact]
    public async Task ClientBlockedPreventsDebit()
    {
        // Arrange
        var account1 = new CreateAccountResponseDto
        {
            OwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Balance = 500,
            InterestRate = 0,
            Currency = IsoCurrency.RUB,
            Type = AccountType.Checking
        };
        
        var account2 = new CreateAccountResponseDto
        {
            OwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Balance = 500,
            InterestRate = 0,
            Currency = IsoCurrency.RUB,
            Type = AccountType.Checking
        };

        // создаём аккаунт
        var createResp1 = await _client.PostAsJsonAsync("/v1/Accounts", account1);
        var createResp2 = await _client.PostAsJsonAsync("/v1/Accounts", account2);
        
        createResp1.EnsureSuccessStatusCode();
        createResp2.EnsureSuccessStatusCode();
        
        var response = await _client.GetAsync("v1/Accounts/owner/11111111-1111-1111-1111-111111111111");
        
        var accounts = await response.Content.ReadFromJsonAsync<List<DbAccount>>();

        Assert.NotNull(accounts);
        var accountId = accounts[0].Id;

        // имитируем событие ClientBlocked
        await _client.PatchAsync("v1/Accounts/owner/11111111-1111-1111-1111-111111111111/blocked/true", null);

        // Act — пробуем списать деньги
        var debitDto = new TransferResponseDto
        {
            AccountId = accountId,
            CounterpartyAccountId = accounts[1].Id,
            Amount = 100m,
            Currency = IsoCurrency.RUB,
            Type = TransactionType.Debit
        };

        var debitResp = await _client.PostAsJsonAsync("v1/Accounts/transfer", debitDto);

        // Assert 1 — HTTP 409
        Assert.Equal(System.Net.HttpStatusCode.Conflict, debitResp.StatusCode);

        // Assert 2 — в outbox нет MoneyDebited
        var messages = await _outboxRepository.TakeDueAsync(50);
        Assert.DoesNotContain(messages, m => m.Type == "MoneyDebited");
    }

    private record AccountDto(Guid Id, decimal Balance);
}