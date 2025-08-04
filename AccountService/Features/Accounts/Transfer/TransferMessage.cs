using AccountService.Common;
using AccountService.Common.Abstractions;
using MediatR;

namespace AccountService.Features.Accounts.Transfer;

public record TransferMessage(TransferResponseDto TransferDto) : IMessage<MbResult<Unit>>;