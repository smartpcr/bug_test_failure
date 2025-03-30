namespace BugAnalysis;

using BugAnalysis.Models;
using ConsoleTables;
using Newtonsoft.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        var bug = new Bug(32051380);
        Console.WriteLine($"Evaluating bug {bug.BugId}...");
        var reproSteps = await bug.DownloadReproSteps();
        var deployments = await Deployment.ParseDeployments(reproSteps);
        Console.WriteLine($"total deployments: {deployments.Count}");
        var failures = deployments
            .SelectMany(d => d.TestFailures.Select(f => new
            {
                d.Environment,
                d.BuildNumber,
                TestFailure = f
            }))
            .GroupBy(f => f.TestFailure.Reason)
            .Select(g => new
            {
                Reason = g.Key.ToString(),
                EnvironmentCount = g.Select(x => x.Environment).Distinct().Count(),
                BuildVersions = string.Join(",", g.Select(x => x.BuildNumber.Version).Distinct()),
                TestFailureCount = g.Count()
            }).ToList();

        var json = JsonConvert.SerializeObject(failures, Formatting.Indented);
        await File.WriteAllTextAsync("output.json", json);

        var table = new ConsoleTable("Reason", "Environment Count", "Build Versions", "Test Failure Count");
        foreach (var failure in failures)
        {
            table.AddRow(failure.Reason, failure.EnvironmentCount, failure.BuildVersions, failure.TestFailureCount);
        }

        table.Write();

        Console.WriteLine("Done!");
    }
}