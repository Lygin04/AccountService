using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace AccountService.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Добавляет поддержку Swagger с авторизацией в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурационные настройки.</param>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Account Service",
                Description = "API микросервиса «Банковские счета», обслуживающий процессы розничного банка",
                Version = "1.1.1.1"
            });

            options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUrl"]!),
                        TokenUrl = new Uri(configuration["Keycloak:TokenUrl"]!),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID scope" },
                            { "profile", "Profile scope" }
                        }
                    }
                }
            });

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Keycloak",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string> { "openid", "profile" }
                }
            };

            options.AddSecurityRequirement(securityRequirement);

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
        services.AddSwaggerExamplesFromAssemblyOf<Program>();
        return services;
    }
    
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddApiControllers(this IServiceCollection services, IWebHostEnvironment environment)
    {
        if (!environment.IsEnvironment("Test"))
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build()));
            }).AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        }
        else
        {
            services.AddControllers().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        }

        return services;
    }
    
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddHealthChecksWithDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSection = configuration.GetSection("RabbitMQ");
        var rabbitMqConnectionString =
            $"amqp://{rabbitMqSection["UserName"]}:{rabbitMqSection["Password"]}@{rabbitMqSection["HostName"]}:{rabbitMqSection["Port"]}/";

        services.AddHealthChecks()
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
            .AddRabbitMQ(
                rabbitMqConnectionString,
                name: "rabbitmq",
                tags: ["ready"])
            .AddNpgSql(
                connectionString: configuration["BankDataBase:ConnectionString"]!,
                name: "postgres",
                tags: ["ready"]);

        return services;
    }
    
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddHangfireWithPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
            config.UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(configuration["BankDataBase:ConnectionString"])));

        services.AddHangfireServer();
        return services;
    }
}