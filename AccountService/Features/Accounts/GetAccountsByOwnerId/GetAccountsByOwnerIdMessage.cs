using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Features.Accounts.Models;

namespace AccountService.Features.Accounts.GetAccountsByOwnerId;

public record GetAccountsByOwnerIdMessage(Guid OwnerId) : IMessage<MbResult<List<DbAccount>>>;