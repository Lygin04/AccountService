using AccountService.Infrastructure.Clients.Interfaces;

namespace AccountService.Infrastructure.Clients;

public class CurrencyServiceStub : ICurrencyService
{
    private static readonly HashSet<string> SupportedCurrencies = ["RUB", "USD", "EUR"];

    public bool IsSupported(string currencyCode)
    {
        return SupportedCurrencies.Contains(currencyCode.ToUpperInvariant());
    }
}