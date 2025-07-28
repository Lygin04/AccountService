using AccountService.Common.Abstractions;
using MediatR;

namespace AccountService.Features.Accounts.Transfer;

public record TransferCommand(TransferResponseDto TransferDto) : ICommand<Unit>;