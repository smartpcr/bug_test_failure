// -----------------------------------------------------------------------
// <copyright file="BuildPipeline.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models
{
	using System.IO.Compression;
	using System.Net.Http.Headers;
	using System.Text;
	using BugAnalysis.Models.Tests;
	using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.VisualStudio.Services.Common;
    using Microsoft.VisualStudio.Services.WebApi;
	using Newtonsoft.Json;

	public class BuildPipeline
    {
        private const string ArtifactName = "ASZ_BVT";
        private const string TestPlanFile = "testPlanReport.json";
        private readonly int buildId;

        public BuildPipeline(int buildId)
        {
            this.buildId = buildId;
        }

        public async Task<List<TestFailure>> GetTestFailures(CancellationToken cancel)
        {
            var pat = Ado.GetPat();
            var connection = new VssConnection(new Uri(Ado.AdoUrl), new VssBasicCredential(string.Empty, pat));
            var buildClient = connection.GetClient<BuildHttpClient>();
            var buildArtifact = await buildClient.GetArtifactAsync(Ado.Project, buildId, BuildPipeline.ArtifactName, cancellationToken: cancel);
            var downloadUrl = buildArtifact.Resource.DownloadUrl;

            using (var client = new HttpClient())
			{
				var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"user:{pat}"));
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
				using (var response = await client.GetAsync(downloadUrl, cancel))
				{
					response.EnsureSuccessStatusCode();
					var downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
					string zipFilePath = Path.Combine(downloadFolder, $"{BuildPipeline.ArtifactName}_{this.buildId}.zip");
					await using (var fs = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
					{
						await response.Content.CopyToAsync(fs);
					}
					Console.WriteLine($"Artifact downloaded and saved to: {zipFilePath}");

					using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
					{
						// Find the file by matching the entry name, which ignores folder paths.
						var entry = archive.Entries.FirstOrDefault(e => e.Name.Equals(BuildPipeline.TestPlanFile, StringComparison.OrdinalIgnoreCase));

						if (entry != null)
						{
							await using var stream = entry.Open();
							using var reader = new StreamReader(stream);
							var testReportJson = await reader.ReadToEndAsync(cancel);
							var array = JsonConvert.DeserializeObject<object[]>(testReportJson);

							// Since the JSON is an array with the first two items as strings and the third as the test plan:
							var root = new TestPlanRoot
							{
								ExtraModules = array[0]?.ToString(),
								MASCIHelpersPath = array[1]?.ToString(),
								TestPlan = JsonConvert.DeserializeObject<TestPlan>(array[2].ToString())
							};

							var testCases = root.TestPlan!.testResults.all.testCaseSummary.ToList();
							var testFailures = testCases.Where(t => !t.Succeeded)
								.Select(t => new TestFailure {
									TestProvider = t.TestProvider,
									TestCase = t.Suite,
									Error = t.ErrorMessages?.FirstOrDefault(),
									IsTimeout = t.TimedOut
								}).ToList();
							return testFailures;
						}

						Console.WriteLine($"Entry '{BuildPipeline.TestPlanFile}' not found in the zip file.");
					}
				}
			}

			return new List<TestFailure>();
        }
    }
}