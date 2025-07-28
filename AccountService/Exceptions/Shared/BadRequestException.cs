namespace AccountService.Exceptions.Shared;

/// <summary>
/// Исключение, указывающее на то, что запрос был неверным.
/// </summary>
public class BadRequestException : Exception
{
    /// <summary>
    /// Создает новый экземпляр исключения с указанным сообщением.
    /// </summary>
    /// <param name="message">Сообщение об ошибке.</param>
    protected BadRequestException(string message) : base(message) { }
}