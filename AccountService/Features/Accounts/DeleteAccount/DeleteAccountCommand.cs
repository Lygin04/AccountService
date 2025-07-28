using AccountService.Common.Abstractions;
using MediatR;

namespace AccountService.Features.Accounts.DeleteAccount;

public record DeleteAccountCommand(Guid Id) : ICommand<Unit>;