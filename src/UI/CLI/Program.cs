﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using SpreaGit.Application.Interfaces;
using SpreaGit.Application.Services;
using SpreaGit.Infrastructure.Readers;
using SpreaGit.Infrastructure.Spreaders;
using SpreaGit.Infrastructure.Writers;

try
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] - {Message:lj}{NewLine}{Exception}")
        .CreateLogger();
    
    Log.Information("Starting SpreaGit");
    Log.Information("Configuring Services");

    var builder = Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddScoped<ISpreaGitService, SpreaGitService>();
            services.AddScoped<IConfigurationReader, JsonConfigurationReader>();
            services.AddScoped<IRepositoryReader, RepositoryReader>();
            services.AddScoped<IRepositoryWriter, RepositoryWriter>();
            services.AddScoped<ICommitDateSpreader, ComplexCommitDateSpreader>();
            // services.AddScoped<ICommitDateSpreader, CommitDateSpreader>();
        })
        .UseSerilog();
    
    using var host = builder.Build();
   
    var spreaGitService = host.Services.GetRequiredService<ISpreaGitService>();
    
    await spreaGitService.SpreaGitAsync();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.ToString());
}
finally
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}