using AccountService.Common.Abstractions;
using AccountService.Contracts;
using AccountService.Features.Accounts.BlockedAccount;
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
    [ProducesResponseType(typeof(AccountOpened), StatusCodes.Status201Created)]
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
    /// <param name="cancellationToken">Токен для отмены асинхронной операции.</param>
    /// <returns>Статус 204 No Content при успешном обновлении, или ошибки.</returns>
    [HttpPatch("{accountId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid accountId, UpdateAccountResponseDto accountDto,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateAccountMessage(accountId, accountDto), cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Удаляет счёт по идентификатору.
    /// </summary>
    /// <param name="accountId">Идентификатор счёта.</param>
    /// <param name="cancellationToken">Токен для отмены асинхронной операции.</param>
    /// <returns>Статус 204 No Content при успешном удалении, или 404 если не найден.</returns>
    [HttpDelete("{accountId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid accountId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteAccountMessage(accountId), cancellationToken);

        return ToActionResult(result);
    }

    /// <summary>
    /// Получает счёт по идентификатору.
    /// </summary>
    /// <param name="accountId">Идентификатор счёта.</param>
    /// <param name="cancellationToken">Токен для отмены асинхронной операции.</param>
    /// <returns>Данные счёта и статус 200 OK, либо 404 если счёт не найден.</returns>
    [HttpGet("{accountId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid accountId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAccountMessage(accountId), cancellationToken);
        
        return ToActionResult(result);
    }

    /// <summary>
    /// Получает выписку всех счетов по идентификатору владельца.
    /// </summary>
    /// <param name="ownerId">Идентификатор владельца.</param>
    /// <param name="cancellationToken">Токен для отмены асинхронной операции.</param>
    /// <returns>Выписка счетов и статус 200 OK, либо 404 если владелец не найден.</returns>
    [HttpGet("owner/{ownerId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByOwnerId(Guid ownerId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAccountsByOwnerIdMessage(ownerId), cancellationToken);
        
        return ToActionResult(result);
    }

    /// <summary>
    /// Переводит средства между счетами.
    /// </summary>
    /// <param name="transferDto">Данные перевода.</param>
    /// <param name="cancellationToken">Токен для отмены асинхронной операции.</param>
    /// <returns>Статус 201 Created при успешном переводе, или ошибки.</returns>
    [HttpPost("transfer")]
    [ProducesResponseType(typeof(AccountOpened), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Transfer(TransferResponseDto transferDto, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new TransferMessage(transferDto), cancellationToken);
        
        return result.IsSuccess
            ? Created("", new { result.IsSuccess, result.Data, result.Error })
            : ToActionResult(result);
    }

    /// <summary>
    /// Блокирует/Разблокирует все счета владельца.
    /// </summary>
    /// <param name="ownerId">Идентификатор владельца.</param>
    /// <param name="isBlocked">true - заблокировать. false - разблокировать.</param>
    /// <param name="cancellationToken">Токен для отмены асинхронной операции.</param>
    [HttpPatch("owner/{ownerId:guid}/blocked/{isBlocked:bool}")]
    [ProducesResponseType(typeof(ClientBlocked), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ClientUnblocked), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetBlockedByOwnerId(Guid ownerId, bool isBlocked,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new BlockedAccountMessage(ownerId, isBlocked), cancellationToken);

        return ToActionResult(result);
    }
}