using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AccountService.Extensions;

/// <summary>
/// Расширения для настройки аутентификации в приложении.
/// </summary>
/// <remarks>
/// Этот класс содержит методы для добавления аутентификации JWT и регистрации компонентов модуля аутентификации в контейнере зависимостей.
/// </remarks>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Добавляет аутентификацию JWT в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = configuration["Authentication:Authority"];
                options.Audience = configuration["Authentication:Audience"];
                //options.MetadataAddress = configuration["Authentication:MetadataAddress"]!;
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Authentication:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Authentication:Audience"],
                    ValidateLifetime = true,
                    ValidAlgorithms = ["RS256"]
                };
            });
    }
}