namespace AccountService.Infrastructure.Clients.Interfaces;

public interface ICurrencyService
{
    bool IsSupported(string currencyCode);
}