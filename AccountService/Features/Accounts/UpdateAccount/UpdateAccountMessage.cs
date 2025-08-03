using AccountService.Common.Abstractions;
using MediatR;

namespace AccountService.Features.Accounts.UpdateAccount;

public record UpdateAccountMessage(Guid AccountId, UpdateAccountResponseDto AccountDto) : IMessage<Unit>;