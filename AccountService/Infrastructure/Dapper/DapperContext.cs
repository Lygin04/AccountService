using System.Data;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Dapper.Models;
using Dapper;

namespace AccountService.Infrastructure.Dapper;

public class DapperContext<TSettings> : IDapperContext<TSettings> where TSettings : IDapperSettings
{
    private readonly string _connectionString;
    private readonly Provider _provider;

    public DapperContext(IDapperSettings settings)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (settings == null)
        {
#pragma warning disable CA2208
            throw new ArgumentException();
#pragma warning restore CA2208
        }

        _connectionString = settings.ConnectionString;
        _provider = settings.Provider;
    }

    public async Task<T?> FirstOrDefault<T>(IQueryObject queryObject)
    {
        return await Execute(query => query.QueryFirstOrDefaultAsync<T>(queryObject.Sql, queryObject.Params, commandTimeout: queryObject.CommandTimeout)).ConfigureAwait(false);
    }

    public async Task<List<T>> ListOrEmpty<T>(IQueryObject queryObject)
    {
        return (await Execute(query => query.QueryAsync<T>(queryObject.Sql, queryObject.Params, commandTimeout: queryObject.CommandTimeout)).ConfigureAwait(false)).AsList();
    }

    public async Task Command(IQueryObject queryObject, ITransaction? transaction = null)
    {
        await (transaction == null ? CommandExecute(queryObject) : transaction.Command(queryObject));
    }

    public async Task<T?> CommandWithResponse<T>(IQueryObject queryObject, ITransaction? transaction = null)
    {
        return await (transaction == null ? CommandWithResponseExecute<T>(queryObject) : transaction.CommandWithResponse<T>(queryObject));
    }

    public ITransaction BeginTransaction()
    {
        return new Transaction(_connectionString, _provider);
    }

    private async Task<T> Execute<T>(Func<IDbConnection, Task<T>> query)
    {
        using var connection = ConnectionFactory.Create(_connectionString, _provider);
        var result = await query(connection).ConfigureAwait(false);

        return result;
    }

    private async Task CommandExecute(IQueryObject queryObject)
    {
        await Execute(query => query.ExecuteAsync(queryObject.Sql, queryObject.Params, commandTimeout: queryObject.CommandTimeout)).ConfigureAwait(false);
    }

    private async Task<T?> CommandWithResponseExecute<T>(IQueryObject queryObject)
    {
        return await Execute(query => 
            query.QueryFirstOrDefaultAsync<T>(queryObject.Sql, queryObject.Params, commandTimeout: queryObject.CommandTimeout)
        ).ConfigureAwait(false);
    }
}