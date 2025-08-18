using AccountService.Common;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace AccountService.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder  app)
    {
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

        return app;
    }

    public static IEndpointRouteBuilder MapSwaggerWithAuthUi(this IEndpointRouteBuilder endpoints, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            endpoints.MapOpenApi();

            var app = (endpoints as IApplicationBuilder) ?? throw new InvalidOperationException();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Account Service");
                options.OAuthClientId("myclient");
                options.OAuthUsePkce();
                options.OAuthScopeSeparator(" ");
            });
        }

        return endpoints;
    }

    public static IEndpointRouteBuilder MapHangfireDashboardWithAuth(this IEndpointRouteBuilder  app)
    {
        app.MapHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = [new AllowAllDashboardAuthorizationFilter()]
        }).AllowAnonymous();

        return app;
    }
}