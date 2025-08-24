using AccountService.Common;
using AccountService.Common.Abstractions;
using MediatR;

namespace AccountService.Features.Accounts.BlockedAccount;

public record BlockedAccountMessage(Guid OwnerId, bool IsBlocked) : IMessage<MbResult<Unit>>;