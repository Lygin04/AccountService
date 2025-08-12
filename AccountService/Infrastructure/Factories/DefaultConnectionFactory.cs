using System.Data.Common;
using AccountService.Infrastructure.Factories.Interfaces;
using Npgsql;

namespace AccountService.Infrastructure.Factories;

public class DefaultConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    public async Task<DbConnection> CreateAsync()
    {
        var connection = new NpgsqlConnection(configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        return connection;
    }
}