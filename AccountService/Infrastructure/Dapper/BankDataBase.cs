using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Dapper.Models;

namespace AccountService.Infrastructure.Dapper;

public class BankDataBase(IConfiguration configuration) : IDapperSettings
{
    public string ConnectionString => configuration["BankDataBase:ConnectionString"] ?? string.Empty;
    public Provider Provider => Enum.Parse<Provider>(configuration["BankDataBase:Provider"] ?? string.Empty);
}