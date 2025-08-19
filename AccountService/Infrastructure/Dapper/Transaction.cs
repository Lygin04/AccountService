using System.Data;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Dapper.Models;
using Dapper;

namespace AccountService.Infrastructure.Dapper;

public class Transaction : ITransaction
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;

    public Transaction(string connectionString, Provider provider)
    {
        _connection = ConnectionFactory.Create(connectionString, provider);

        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }

        _transaction = _connection.BeginTransaction();
    }

#pragma warning disable CA1816
    public void Dispose()
#pragma warning restore CA1816
    {
        _transaction.Dispose();
        _connection.Close();
        _connection.Dispose();
    }

    public void Commit()
    {
        _transaction.Commit();
    }

    public void Rollback()
    {
        _transaction.Rollback();
    }

    public async Task Command(IQueryObject queryObject)
    {
        await _connection
            .ExecuteAsync(queryObject.Sql, queryObject.Params, _transaction, queryObject.CommandTimeout)
            .ConfigureAwait(false);
    }

    public async Task<T?> CommandWithResponse<T>(IQueryObject queryObject)
    {
        return await _connection
            .QueryFirstOrDefaultAsync<T>(queryObject.Sql, queryObject.Params, _transaction, queryObject.CommandTimeout)
            .ConfigureAwait(false);
    }
}