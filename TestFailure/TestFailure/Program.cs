namespace BugAnalysis;

using BugAnalysis.Models;
using ConsoleTables;
using Newtonsoft.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        var bug = new Bug(32150896);
        Console.WriteLine($"Evaluating bug {bug.BugId}...");
        var reproSteps = await bug.DownloadReproSteps();
        var deployments = await Deployment.ParseDeployments(reproSteps);
        Console.WriteLine($"total deployments: {deployments.Count}");

        // filter out deployments by test provider: Download, Update
        foreach (var deployment in deployments)
        {
            deployment.TestFailures = deployment.TestFailures
                .Where(x => x.TestProvider == "Update" || x.TestProvider == "Download").ToList();
        }
        deployments = deployments
            .Where(x => x.TestFailures.Count > 0)
            .ToList();

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
                Reason = g.Key,
                EnvironmentCount = g.Select(x => x.Environment).Distinct().Count(),
                BuildVersions = string.Join(",", g.Select(x => x.BuildNumber.Version).Distinct()),
                TestFailureCount = g.Count()
            }).ToList();

        var json = JsonConvert.SerializeObject(deployments, Formatting.Indented);
        await File.WriteAllTextAsync("deployments.json", json);

        var table = new ConsoleTable("Reason", "Environment Count", "Build Versions", "Test Failure Count");
        foreach (var failure in failures)
        {
            table.AddRow(failure.Reason, failure.EnvironmentCount, failure.BuildVersions, failure.TestFailureCount);
        }

        table.Write();

        Console.WriteLine("Done!");
    }
}