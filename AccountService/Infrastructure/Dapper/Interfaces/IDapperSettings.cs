using AccountService.Infrastructure.Dapper.Models;

namespace AccountService.Infrastructure.Dapper.Interfaces;

public interface IDapperSettings
{
    string ConnectionString { get; }
    Provider Provider { get; }
}