using AccountService.Common.Abstractions;
using MediatR;

namespace AccountService.Features.Accounts.CreateAccount;

public record CreateAccountCommand(CreateAccountResponseDto CreateAccountResponseDto) : ICommand<Unit>;