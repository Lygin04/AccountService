using MediatR;

namespace AccountService.Common.Abstractions;

public interface IMessage<out TResponse> : IRequest<TResponse>;