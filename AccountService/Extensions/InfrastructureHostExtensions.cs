using AccountService.Behaviors;
using AccountService.Features.Accounts;
using AccountService.Features.Transactions;
using AccountService.Infrastructure.Clients;
using AccountService.Infrastructure.Clients.Interfaces;
using AccountService.Infrastructure.Dapper;
using AccountService.Infrastructure.Dapper.Interfaces;
using AccountService.Infrastructure.Factories;
using AccountService.Infrastructure.Factories.Interfaces;
using AccountService.Infrastructure.Outbox;
using AccountService.Infrastructure.Outbox.Interfaces;
using AccountService.Infrastructure.RabbitMq;
using AccountService.Infrastructure.RabbitMq.Interfaces;
using DbUp;
using FluentValidation;
using MediatR;

namespace AccountService.Extensions;

/// <summary>
/// Расширения для настройки инфраструктурных компонентов приложения.
/// </summary>
/// <remarks>
/// Этот класс содержит методы для настройки базы данных и регистрации репозиториев и фабрик в контейнере зависимостей.
/// </remarks>
public static class InfrastructureHostExtensions
{
    /// <summary>
    /// Выполняет миграцию базы данных для указанного контекста.
    /// </summary>
    public static void MigrateDatabase(IConfiguration configuration)
    {
        var connectionString = configuration["BankDataBase:ConnectionString"];

        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DapperContext<>).Assembly)
            .WithTransaction()
            .WithVariablesDisabled()
            .LogToConsole()
            .Build();

        if (upgrader.IsUpgradeRequired())
            upgrader.PerformUpgrade();
    }
    
    /// <summary>
    /// Подключение Даппера.
    /// </summary>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddDapper(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDapperSettings, BankDataBase>()
            .AddSingleton<IDapperContext<IDapperSettings>, DapperContext<IDapperSettings>>();
    }

    /// <summary>
    /// Подключение и настройка библиотеки FluentValidation.
    /// </summary>
    /// <param name="services"></param>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    /// <summary>
    /// Добавляет инфраструктурные сервисы в коллекцию сервисов.
    /// </summary>
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDbConnectionFactory, DefaultConnectionFactory>();
        
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<ITransactionRepository, TransactionRepository>();
        
        services.AddSingleton<IRabbitMqConnection>(new RabbitMqConnection(configuration));

        services.AddTransient<IOutboxRepository, OutboxRepository>();
        services.AddSingleton<IOutboxWriter, OutboxWriter>();
        services.AddSingleton<OutboxDispatcher>();
        
        services.AddSingleton<IInboxRepository, InboxRepository>();
        
        services.AddSingleton<IClientVerificationService, ClientVerificationStub>();
        services.AddSingleton<ICurrencyService, CurrencyServiceStub>();
    }
}