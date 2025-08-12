using AccountService.Extensions;
using AccountService.Infrastructure;
using AccountService.Infrastructure.Clients;
using AccountService.Infrastructure.Clients.Interfaces;
using AccountService.Middleware;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddSwaggerWithAuth(configuration);

    builder.Services.AddJwtAuth(configuration);
    builder.Services.AddAuthorization();
}

builder.Services.MigrateDatabase(configuration);
builder.Services.AddDapper();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
builder.Services.AddInfrastructure();
builder.Services.AddSingleton<IClientVerificationService, ClientVerificationStub>();
builder.Services.AddSingleton<ICurrencyService, CurrencyServiceStub>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddFluentValidation();

if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new AuthorizeFilter());
    });
}
else
{
    builder.Services.AddControllers();
}

builder.Services.AddHangfire(config =>
{
#pragma warning disable CS0618 // Type or member is obsolete
    config.UsePostgreSqlStorage(configuration["BankDataBase:ConnectionString"]);
#pragma warning restore CS0618 // Type or member is obsolete
});

builder.Services.AddHangfireServer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<AccrueInterestHandler>(
        "accrue-interest-daily",
        handler => handler.HandleAsync(),
        Cron.Daily(0, 0),
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time")
        });
}

app.UseCors(cors =>
{
    cors.AllowAnyHeader();
    cors.AllowAnyMethod();
    cors.AllowAnyOrigin();
});

if (!app.Environment.IsEnvironment("Test"))
{
    app.UseAuthentication();
    app.UseAuthorization();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Account Service");

        options.OAuthClientId("myclient");
        options.OAuthUsePkce();
        options.OAuthScopeSeparator(" ");
    });
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();

#pragma warning disable CA1050
public abstract class ProgramPlaceholder;
#pragma warning restore CA1050

