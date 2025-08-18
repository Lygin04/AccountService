using AccountService.Extensions;
using AccountService.Infrastructure;
using AccountService.Infrastructure.Outbox;
using AccountService.Middleware;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
InfrastructureHostExtensions.MigrateDatabase(configuration);
builder.Services.AddDapper();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
builder.Services.AddInfrastructure(configuration);
builder.Services.AddFluentValidation();

if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddSwaggerWithAuth(configuration);

    builder.Services.AddJwtAuth(configuration);
    builder.Services.AddAuthorization();
}

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddApiControllers(configuration, builder.Environment);
builder.Services.AddHealthChecksWithDependencies(configuration);
builder.Services.AddHangfireWithPostgres(configuration);

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

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<HttpLoggingMiddleware>();

app.MapControllers();
app.MapHealthEndpoints();
app.MapSwaggerWithAuthUi(app.Environment);
app.MapHangfireDashboardWithAuth();

app.Run();

#pragma warning disable CA1050
public abstract class ProgramPlaceholder;
#pragma warning restore CA1050