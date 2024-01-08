﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace dk.roderos.SpreaGit.Application;

public class SpreaGitService(
    IConfiguration configuration,
    ILogger<SpreaGitService> logger,
    IConfigurationReader configurationReader,
    IRepositoryReader repositoryReader,
    IRepositoryWriter repositoryWriter) : ISpreaGitService
{
    public async Task SpreaGitAsync()
    {
        var configFile = configuration.GetSection("config").Value;

        logger.LogInformation("Config File: {configFile}", configFile);

        if (string.IsNullOrEmpty(configFile))
        {
            logger.LogError("Must specify 'config' argument");

            return;
        }

        if (!File.Exists(configFile))
        {
            logger.LogError("File does not exists: {configFile}", configFile);

            return;
        }

        var spreaGitConfiguration = await configurationReader.ReadConfigurationAsync(configFile);

        logger.LogInformation("SpreaGit Configuration: {spreaGitConfiguration}", spreaGitConfiguration);

        var repositoryPath = spreaGitConfiguration!.RepositoryPath;

        if (!Directory.Exists(repositoryPath))
        {
            logger.LogError("Directory does not exists: {repositoryPath}", repositoryPath);

            return;
        }

        // We need to start at the initial commit so we should reverse the list
        var commits = repositoryReader.GetGitCommits(repositoryPath).ToList();
        commits.Reverse();

        logger.LogInformation("Logs count: {commitsCount}", commits.Count);

        var outputPath = spreaGitConfiguration.OutputPath;

        logger.LogInformation("Output Path: {outputPath}", repositoryPath);

        var outputRepositoryName = new DirectoryInfo(repositoryPath).Name + " spreagit";
        var outputRepositoryPath = Path.Combine(outputPath, outputRepositoryName);

        logger.LogInformation("Output Repository Path: {outputPath}", repositoryPath);

        var suffix = 1;
        while (Directory.Exists(outputRepositoryPath))
        {
            outputRepositoryPath = Path.Combine(outputPath, $"{outputRepositoryName} ({suffix})");
            suffix++;
        }

        Directory.CreateDirectory(outputRepositoryPath);

        repositoryWriter.InitializeGit(outputRepositoryPath);

        foreach (var commit in commits)
        {
            logger.LogInformation("Checking out commit: {commitId}", commit.Id);
            repositoryReader.CheckoutCommit(repositoryPath, commit.Id);
        }
    }
}