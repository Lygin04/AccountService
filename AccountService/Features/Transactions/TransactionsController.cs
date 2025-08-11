using AccountService.Common;
using AccountService.Features.Transactions.CreateTransaction;
using AccountService.Features.Transactions.GetByAccountIdTransaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transactions;

public class TransactionsController(IMediator mediator) : ApiControllerV1WithAuth
{
    /// <summary>
    /// Создает транзакцию из внешнего сервиса.
    /// </summary>
    /// <param name="transactionDto">Данные для создания транзакции.</param>
    /// <returns>Статус 201 Created при успешном создании, или ошибки.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(TransactionDto transactionDto)
    {
        var result = await mediator.Send(new CreateTransactionMessage(transactionDto));
        
        return result.IsSuccess
            ? Created("", new { result.IsSuccess, result.Data, result.Error })
            : ToActionResult(result);
    }

    /// <summary>
    /// Получает список транзакций по идентификатору счета.
    /// </summary>
    /// <param name="accountId">Идентификатор счета.</param>
    /// <returns>Список транзакций и статус 200 OK, либо 404 если счёт не найден</returns>
    [HttpGet("{accountId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByAccountId(Guid accountId)
    {
        var result = await mediator.Send(new GetByAccountIdTransactionMessage(accountId));
        
        return ToActionResult(result);
    }
}