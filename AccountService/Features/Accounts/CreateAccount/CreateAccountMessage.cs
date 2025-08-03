using AccountService.Common.Abstractions;
using MediatR;

namespace AccountService.Features.Accounts.CreateAccount;

public record CreateAccountMessage(CreateAccountResponseDto CreateAccountResponseDto) : IMessage<Unit>;