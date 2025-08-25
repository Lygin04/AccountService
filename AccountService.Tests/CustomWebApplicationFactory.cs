using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AccountService.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class CustomWebApplicationFactory<TProgram>(string postgresConnectionString, string rabbitHost, string rabbitPort)
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.UseSetting("BankDataBase:ConnectionString", postgresConnectionString);
        builder.UseSetting("BankDataBase:Provider", "PostgreSQL");

        builder.UseSetting("RabbitMQ:HostName", rabbitHost);
        builder.UseSetting("RabbitMQ:Port", rabbitPort);
        builder.UseSetting("RabbitMQ:UserName", "guest");
        builder.UseSetting("RabbitMQ:Password", "guest");
    }
}