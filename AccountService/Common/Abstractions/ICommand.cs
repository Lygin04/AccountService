using MediatR;

namespace AccountService.Common.Abstractions;

public interface ICommand<out TResponse> : IRequest<TResponse>;