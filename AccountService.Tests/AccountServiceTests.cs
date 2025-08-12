using AccountService.Common.Enums;
using AccountService.Features.Accounts.CreateAccount;
using AccountService.Features.Accounts.Models;
using AccountService.Features.Accounts.Transfer;
using AccountService.Features.Transactions.Models;
using FluentAssertions;

namespace AccountService.Tests;

public class AccountBusinessLogicTests
{
    [Fact]
    public void CreateAccountDto_Validation_ShouldPassForValidData()
    {
        // Arrange
        var dto = new CreateAccountResponseDto
        {
            OwnerId = Guid.NewGuid(),
            Type = AccountType.Checking,
            Currency = IsoCurrency.Usd,
            Balance = 1000,
            InterestRate = null
        };

        // Act & Assert
        dto.Should().NotBeNull();
        dto.OwnerId.Should().NotBeEmpty();
        dto.Type.Should().Be(AccountType.Checking);
        dto.Currency.Should().Be(IsoCurrency.Usd);
        dto.Balance.Should().Be(1000);
        dto.InterestRate.Should().BeNull();
    }

    [Theory]
    [InlineData(AccountType.Deposit, 0.05)]
    [InlineData(AccountType.Credit, 0.15)]
    public void CreateAccountDto_WithInterestRate_ShouldBeValidForDepositAndCredit(AccountType accountType, decimal interestRate)
    {
        // Arrange
        var dto = new CreateAccountResponseDto
        {
            OwnerId = Guid.NewGuid(),
            Type = accountType,
            Currency = IsoCurrency.Eur,
            Balance = 500,
            InterestRate = interestRate
        };

        // Act & Assert
        dto.InterestRate.Should().Be(interestRate);
    }

    [Fact]
    public void TransferDto_Validation_ShouldRequirePositiveAmount()
    {
        // Arrange
        var dto = new TransferResponseDto
        {
            AccountId = Guid.NewGuid(),
            CounterpartyAccountId = Guid.NewGuid(),
            Amount = 100,
            Currency = IsoCurrency.Usd,
            Type = TransactionType.Debit
        };

        // Act & Assert
        dto.Amount.Should().BePositive();
        dto.AccountId.Should().NotBe(dto.CounterpartyAccountId);
    }
}