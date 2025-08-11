using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Dapper.Models;

namespace AccountService.Infrastructure.Dapper;

public class BankDataBase : IDapperSettings
{
    private readonly IConfiguration _configuration;
    
    public BankDataBase(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string ConnectionString => _configuration["BankDataBase:ConnectionString"] ?? string.Empty;
    public Provider Provider => Enum.Parse<Provider>(_configuration["BankDataBase:Provider"] ?? string.Empty);
}