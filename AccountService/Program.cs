using System.Text.Json;
using System.Text.Json.Serialization;
using AccountService.Common;
using AccountService.Extensions;
using AccountService.Infrastructure;
using AccountService.Infrastructure.Clients;
using AccountService.Infrastructure.Clients.Interfaces;
using AccountService.Infrastructure.Outbox;
using AccountService.Infrastructure.RabbitMq;
using AccountService.Infrastructure.RabbitMq.Interfaces;
using AccountService.Middleware;
using Hangfire;
using Hangfire.PostgreSql;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
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

builder.Services.AddSingleton<IRabbitMqConnection>(new RabbitMqConnection(configuration));

var rabbitMqSection = builder.Configuration.GetSection("RabbitMQ");
var rabbitMqConnectionString =
    $"amqp://{rabbitMqSection["UserName"]}:{rabbitMqSection["Password"]}@{rabbitMqSection["HostName"]}:{rabbitMqSection["Port"]}/";


builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
    .AddRabbitMQ(
        rabbitMqConnectionString,
        name: "rabbitmq",
        tags: ["ready"])
    .AddNpgSql(
        connectionString: configuration["BankDataBase:ConnectionString"]!,
        name: "postgres",
        tags: ["ready"]);

if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddControllers(options =>
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
    builder.Services.AddControllers().AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
}

builder.Services.AddHangfire(config =>
    config.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(configuration["BankDataBase:ConnectionString"])));

builder.Services.AddHangfireServer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var recurringJobs = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobs.AddOrUpdate<AccrueInterestHandler>(
        "accrue-interest-daily",
        handler => handler.HandleAsync(),
        Cron.Daily(0, 0),
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time")
        });

    recurringJobs.AddOrUpdate<OutboxDispatcher>(
        "outbox-dispatcher",
        d => d.DispatchBatch(),
        "*/10 * * * * *");
    
    recurringJobs.AddOrUpdate<InboxConsumer>(
        "inbox-consumer",
        x => x.ConsumeAsync(),
        "*/1 * * * * *");
}

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Name == "self",
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();

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

app.MapHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new AllowAllDashboardAuthorizationFilter()]
}).AllowAnonymous();

app.UseMiddleware<HttpLoggingMiddleware>();

app.MapControllers();
app.Run();

#pragma warning disable CA1050
public abstract class ProgramPlaceholder;
#pragma warning restore CA1050