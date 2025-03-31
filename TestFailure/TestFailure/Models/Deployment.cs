// -----------------------------------------------------------------------
// <copyright file="Deployment.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models
{
	using System.Collections.Concurrent;
	using System.Globalization;
	using System.Text.RegularExpressions;

	public class Deployment
    {
        public string Environment { get; set; }
        public string IPAddress { get; set; }
        public string LogShare { get; set; }
        public string PipelineLink { get; set; }
        public DateTime Date { get; set; }
        public BuildNumber BuildNumber { get; set; }
        public string AeoDeviceUri { get; set; }
        public List<TestFailure> TestFailures { get; set; }

        public static async Task<List<Deployment>> ParseDeployments(string outputContent)
        {
	        string pattern = @"\s*\[Triaged.*?\]";
	        outputContent = Regex.Replace(outputContent, pattern, string.Empty, RegexOptions.IgnoreCase);
	        outputContent =string.Join('\n', outputContent.Split(
			        new[] { '\n' },
			        StringSplitOptions.RemoveEmptyEntries)
		        .Where(line => !string.IsNullOrWhiteSpace(line)));
	        var environmentContents = RegexPatterns.environmentSplit.Split(outputContent);
	        var environmentContentList = environmentContents.Select(ec => ec.Trim()).Where(ec => !string.IsNullOrEmpty(ec)).ToList();

	        var deployments = new ConcurrentBag<Deployment>();
	        await Parallel.ForEachAsync(
		        environmentContentList,
		        new ParallelOptions
		        {
			        MaxDegreeOfParallelism = System.Environment.ProcessorCount
		        },
		        async (envContent, cancel) =>
		        {
			        var deployment = await Deployment.ParseDeployment(envContent, cancel);
			        if (!string.IsNullOrEmpty(deployment.Environment))
			        {
				        deployments.Add(deployment);
			        }
		        });

	        return deployments.ToList();
        }

        public static async Task<Deployment> ParseDeployment(string envContent, CancellationToken cancel)
        {
			var deployment = new Deployment();
			var envNameMatch = RegexPatterns.environmentRegex.Match(envContent);
			if (envNameMatch.Success)
			{
				deployment.Environment = envNameMatch.Groups[1].Value;
			}
			var ipMatch = RegexPatterns.ipRegex.Match(envContent);
			if (ipMatch.Success)
			{
				deployment.IPAddress = ipMatch.Groups[1].Value;
			}

			var logShareMatch = RegexPatterns.logShareRegex.Match(envContent);
			if (logShareMatch.Success)
			{
				deployment.LogShare = logShareMatch.Groups[1].Value;
			}

			var pipelineLinkMatch = RegexPatterns.pipelineLinkRegex.Match(envContent);
			if (pipelineLinkMatch.Success)
			{
				deployment.PipelineLink = pipelineLinkMatch.Groups[1].Value;
			}

			var dateMatch = RegexPatterns.dateRegex.Match(envContent);
			if (dateMatch.Success)
			{
				deployment.Date = DateTime.ParseExact(dateMatch.Groups[1].Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			}

			var buildNumberMatch = RegexPatterns.buildNumberRegex.Match(envContent);
			if (buildNumberMatch.Success)
			{
				var buildString = buildNumberMatch.Groups[1].Value.Trim();
				var buildParts = buildString.Split("_", StringSplitOptions.RemoveEmptyEntries);
				if (buildParts.Length == 3)
				{
					var buildNumber = new BuildNumber();
					buildNumber.Name = buildParts[0];
					buildNumber.Version = buildParts[1];
					buildNumber.BuildTime = DateTime.ParseExact(buildParts[2], "yyyyMMdd-HHmm", CultureInfo.InvariantCulture);
					deployment.BuildNumber = buildNumber;
				}
			}

			var aeoDeviceUriMatch = RegexPatterns.aeoDeviceUriRegex.Match(envContent);
			if (aeoDeviceUriMatch.Success)
			{
				deployment.AeoDeviceUri = aeoDeviceUriMatch.Groups[1].Value;
			}

			deployment.TestFailures = new List<TestFailure>();
			var rpIndex = envContent.IndexOf(RegexPatterns.updateTestRegex, StringComparison.OrdinalIgnoreCase);
			if (rpIndex > 0)
            {
                var testProvider = "Update";
				var rpContent = envContent.Substring(rpIndex + RegexPatterns.updateTestRegex.Length).Trim();
				var suiteTestIndex = rpContent.IndexOf("Suite: Test", StringComparison.OrdinalIgnoreCase);
				if (suiteTestIndex >= 0)
				{
					rpContent = rpContent.Substring(suiteTestIndex + "Suite: Test".Length).Trim();
				}
				var testContents = RegexPatterns.testSplit.Split(rpContent)
					.Select(t => t.Trim())
					.Where(t => !string.IsNullOrEmpty(t)).ToList();
				for (var k = 0; k < testContents.Count; k++)
				{
					var testCase = testContents[k];
					k++;
					var testContent = testContents[k];
					var testFailure = TestFailure.ParseTestFailure(testProvider, testCase, testContent);
					deployment.TestFailures.Add(testFailure);
				}
			}

			if (deployment.TestFailures.Count == 0 && !string.IsNullOrEmpty(deployment.PipelineLink))
			{
				var buildIdMatch = RegexPatterns.buildIdRegex.Match(deployment.PipelineLink);
				if (buildIdMatch.Success)
				{
					var buildId = int.Parse(buildIdMatch.Groups[1].Value);
					var buildPipeline = new BuildPipeline(buildId);
					var testFailures = await buildPipeline.GetTestFailures(deployment, cancel);
					deployment.TestFailures = testFailures;
				}
			}

			return deployment;
        }
    }
}