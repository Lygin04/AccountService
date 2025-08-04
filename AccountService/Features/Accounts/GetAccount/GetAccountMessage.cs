using AccountService.Common;
using AccountService.Common.Abstractions;
using AccountService.Features.Accounts.Models;

namespace AccountService.Features.Accounts.GetAccount;

public record GetAccountMessage(Guid Id) : IMessage<MbResult<Account>>;