using System.Data.Common;

namespace AccountService.Infrastructure.Factories.Interfaces;

/// <summary>
/// Представляет интерфейс фабрики подключений к базе данных.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Создает асинхронное подключение к базе данных.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    Task<DbConnection> CreateAsync();
}