using MediatR;

namespace AccountService.Common.Abstractions;

public interface IQuery<out TResponse> : IRequest<TResponse>;