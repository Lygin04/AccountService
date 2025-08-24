using AccountService.Features.Accounts.Models;
using AccountService.Features.Accounts.UpdateAccount;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Dapper.Models;
using AccountService.Infrastructure.Scripts.Account;

namespace AccountService.Features.Accounts;

public class AccountRepository(IDapperContext<IDapperSettings> dapperContext) : IAccountRepository
{
    public async Task AddAsync(DbAccount dbAccount, ITransaction? transaction = null)
    {
        await dapperContext.Command(new QueryObject(Account.Create, dbAccount), transaction);
    }

    public async Task<bool> UpdateAsync(Guid accountId, UpdateAccountResponseDto accountDto, ITransaction? transaction = null)
    {
        var parameters = new { Id = accountId, accountDto.Balance, accountDto.InterestRate, accountDto.Xmin };
        
        var affectedRows = await dapperContext.CommandWithResponse<int>(
            new QueryObject(Account.Update, parameters),
            transaction);

        return affectedRows == 1;
    }

    public async Task DeleteAsync(Guid accountId, ITransaction? transaction = null)
    {
        await dapperContext.Command(new QueryObject(Account.Delete, new { Id = accountId }), transaction);
    }

    public async Task<DbAccount?> GetByIdAsync(Guid id)
    {
        return await dapperContext.FirstOrDefault<DbAccount>(new QueryObject(Account.GetById, new { Id = id }));
    }

    public async Task<List<DbAccount>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await dapperContext.ListOrEmpty<DbAccount>(new QueryObject(Account.GetByOwnerId, new { OwnerId = ownerId }));
    }

    public async Task<List<Guid>> GetAllAccountIdsAsync()
    {
        return await dapperContext.ListOrEmpty<Guid>(new QueryObject(Account.GetAll));
    }

    public async Task<bool> ExistsAsync(Guid accountId)
    {
        return await dapperContext.CommandWithResponse<bool>(
            new QueryObject(Account.ExistsById, new { Id = accountId }));
    }

    public async Task SetFrozenByOwnerAsync(Guid ownerId, bool frozen, ITransaction? transaction = null)
    {
        await dapperContext.Command(new QueryObject(Account.SetFrozenByOwner,
            new { OwnerId = ownerId, Frozen = frozen }));
    }
}