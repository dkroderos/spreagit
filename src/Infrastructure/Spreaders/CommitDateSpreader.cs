﻿using SpreaGit.Application.Interfaces;
using SpreaGit.Domain.Models;

namespace SpreaGit.Infrastructure.Spreaders;

public class CommitDateSpreader : ICommitDateSpreader
{
    public IEnumerable<GitLog> SpreadOutDateCommits(List<GitLog> gitLogs, SpreaGitConfiguration configuration)
    {
        // var startDate = configuration.StartDate;
        // var endDate = configuration.EndDate;
        
        var alteredCommits = new List<GitLog>();
        alteredCommits.AddRange(gitLogs);

        // var committingHours = DateUtility.GetDateTimeOffsets(startDate, endDate);

        return alteredCommits;
    }
}