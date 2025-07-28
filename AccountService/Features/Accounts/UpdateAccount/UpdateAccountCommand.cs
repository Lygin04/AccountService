using AccountService.Common.Abstractions;
using MediatR;

namespace AccountService.Features.Accounts.UpdateAccount;

public record UpdateAccountCommand(Guid AccountId, UpdateAccountResponseDto AccountDto) : ICommand<Unit>;