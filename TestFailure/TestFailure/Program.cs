namespace BugAnalysis;

using BugAnalysis.Models;
using ConsoleTables;
using Newtonsoft.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        var bug = new Bug(32151219);
        Console.WriteLine($"Evaluating bug {bug.BugId}...");
        await PrintWorkItemFieldNames(bug.BugId);

        var deployments = new List<Deployment>();
        var reproSteps = await bug.DownloadReproSteps();
        var systemInfo = await bug.DownloadSystemInfo();
        if (!string.IsNullOrEmpty(reproSteps))
        {
            deployments = await Deployment.ParseDeployments(reproSteps);
        }

        if (!deployments.Any())
        {
            deployments = await Deployment.ParseDeployments(systemInfo);
        }
        else
        {
            Console.WriteLine("No repro steps or system info found.");
            return;
        }

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

    private static async Task PrintWorkItemFieldNames(int bugId)
    {
        // var bug = new Bug(31298515);
        var bug = new Bug(32151219);
        var results = await bug.FindFieldContaining("s46r07b4");
        foreach (var result in results)
        {
            Console.WriteLine($"Field Name: {result.fieldName}");
            Console.WriteLine($"Field Content: {result.fieldContent}");
        }
    }
}