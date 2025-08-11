using AccountService.Common;
using AccountService.Features.Accounts.CreateAccount;
using AccountService.Features.Accounts.DeleteAccount;
using AccountService.Features.Accounts.GetAccount;
using AccountService.Features.Accounts.GetAccountsByOwnerId;
using AccountService.Features.Accounts.Transfer;
using AccountService.Features.Accounts.UpdateAccount;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Features.Accounts;

public class AccountsController(IMediator mediator) : ApiControllerV1WithAuth
{
    /// <summary>
    /// Создаёт новый счёт.
    /// </summary>
    /// <param name="createAccountResponseDto">Данные для создания счёта.</param>
    /// <returns>Статус 201 Created при успешном создании, или ошибки.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(CreateAccountResponseDto createAccountResponseDto)
    {
        var result = await mediator.Send(new CreateAccountMessage(createAccountResponseDto));

        return result.IsSuccess
            ? Created("", new { result.IsSuccess, result.Data, result.Error })
            : ToActionResult(result);
    }

    /// <summary>
    /// Обновляет существующий счёт по идентификатору.
    /// </summary>
    /// <param name="accountId">Идентификатор счёта.</param>
    /// <param name="accountDto">Данные для обновления счёта.</param>
    /// <returns>Статус 204 No Content при успешном обновлении, или ошибки.</returns>
    [HttpPatch("{accountId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid accountId, UpdateAccountResponseDto accountDto)
    {
        var result = await mediator.Send(new UpdateAccountMessage(accountId, accountDto));
        
        return ToActionResult(result);
    }

    /// <summary>
    /// Удаляет счёт по идентификатору.
    /// </summary>
    /// <param name="accountId">Идентификатор счёта.</param>
    /// <returns>Статус 204 No Content при успешном удалении, или 404 если не найден.</returns>
    [HttpDelete("{accountId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid accountId)
    {
        var result = await mediator.Send(new DeleteAccountMessage(accountId));

        return ToActionResult(result);
    }

    /// <summary>
    /// Получает счёт по идентификатору.
    /// </summary>
    /// <param name="accountId">Идентификатор счёта.</param>
    /// <returns>Данные счёта и статус 200 OK, либо 404 если счёт не найден.</returns>
    [HttpGet("{accountId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid accountId)
    {
        var result = await mediator.Send(new GetAccountMessage(accountId));
        
        return ToActionResult(result);
    }

    /// <summary>
    /// Получает выписку всех счетов по идентификатору владельца.
    /// </summary>
    /// <param name="ownerId">Идентификатору владельца.</param>
    /// <returns>Выписка счетов и статус 200 OK, либо 404 если владелец не найден.</returns>
    [HttpGet("owner/{ownerId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByOwnerId(Guid ownerId)
    {
        var result = await mediator.Send(new GetAccountsByOwnerIdMessage(ownerId));
        
        return ToActionResult(result);
    }

    /// <summary>
    /// Переводит средства между счетами.
    /// </summary>
    /// <param name="transferDto">Данные перевода.</param>
    /// <returns>Статус 201 Created при успешном переводе, или ошибки.</returns>
    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Transfer(TransferResponseDto transferDto)
    {
        var result = await mediator.Send(new TransferMessage(transferDto));
        
        return result.IsSuccess
            ? Created("", new { result.IsSuccess, result.Data, result.Error })
            : ToActionResult(result);
    }
}