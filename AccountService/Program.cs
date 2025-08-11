using AccountService.Behaviors;
using AccountService.Extensions;
using AccountService.Infrastructure.Clients;
using AccountService.Infrastructure.Clients.Interfaces;
using AccountService.Infrastructure.Repositories;
using AccountService.Infrastructure.Repositories.Interfaces;
using AccountService.Middleware;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerWithAuth(configuration);
builder.Services.AddJwtAuth(configuration);
builder.Services.AddAuthorization();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
builder.Services.AddSingleton<FakeDataStorage>();
builder.Services.AddSingleton<IClientVerificationService, ClientVerificationStub>();
builder.Services.AddSingleton<ICurrencyService, CurrencyServiceStub>();
builder.Services.AddSingleton<IFakeDataStorage, FakeDataStorage>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(cors =>
{
    cors.AllowAnyHeader();
    cors.AllowAnyMethod();
    cors.AllowAnyOrigin();
});

app.UseAuthentication();
app.UseAuthorization();

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