using AccountService.Features.Transactions.CreateTransaction;
using AccountService.Features.Transactions.GetByAccountIdTransaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Transactions;

[ApiController]
[Route("v1/[controller]")]
public class TransactionsController(IMediator mediator) : ControllerBase
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
        await mediator.Send(new CreateTransactionMessage(transactionDto));
        
        return Created();
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
        var transactions = await mediator.Send(new GetByAccountIdTransactionMessage(accountId));
        
        return Ok(transactions);
    }
}