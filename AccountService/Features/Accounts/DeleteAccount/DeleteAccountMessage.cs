using AccountService.Common;
using AccountService.Common.Abstractions;
using MediatR;

namespace AccountService.Features.Accounts.DeleteAccount;

public record DeleteAccountMessage(Guid Id) : IMessage<MbResult<Unit>>;