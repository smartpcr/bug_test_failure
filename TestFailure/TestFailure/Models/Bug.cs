// -----------------------------------------------------------------------
// <copyright file="Bug.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models
{
    using System.Net;
    using System.Text.RegularExpressions;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
    using Microsoft.VisualStudio.Services.Common;
    using Microsoft.VisualStudio.Services.WebApi;

    public class Bug
    {
        public int BugId { get; }

        public Bug(int bugId)
        {
            this.BugId = bugId;
        }

        public async Task<List<string>> GetFields()
        {
            var pat = Ado.GetPat();
            var connection = new VssConnection(new Uri(Ado.AdoUrl), new VssBasicCredential(string.Empty, pat));
            var witHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var workItem = await witHttpClient.GetWorkItemAsync(this.BugId);
            Console.WriteLine($"Fields for Work Item ID {this.BugId}:");
            var output = new List<string>();
            foreach (var field in workItem.Fields)
            {
                output.Add($"{field.Key}: {field.Value}");
            }
            return output;
        }

        public async Task<List<(string fieldName, string fieldContent)>> FindFieldContaining(string pattern)
        {
            var pat = Ado.GetPat();
            var connection = new VssConnection(new Uri(Ado.AdoUrl), new VssBasicCredential(string.Empty, pat));
            var witHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var workItem = await witHttpClient.GetWorkItemAsync(this.BugId);
            var fieldNames = workItem.Fields.Keys.ToList();
            var results = new List<(string fieldName, string fieldContent)>();
            foreach (var fieldName in fieldNames)
            {
                if (workItem.Fields.TryGetValue(fieldName, out var fieldContent))
                {
                    var content = fieldContent?.ToString();
                    if (content != null && Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase))
                    {
                        results.Add((fieldName, content));
                    }
                }
            }

            return results;
        }

        public async Task<string> DownloadReproSteps()
        {
            return await this.DownloadWorkItemHtmlField(Ado.ReproStepsField);
        }

        public async Task<string> DownloadSystemInfo()
        {
            return await this.DownloadWorkItemHtmlField(Ado.SystemInfoField);
        }

        public async Task<string> DownloadWorkItemHtmlField(string fieldName)
        {
            var pat = Ado.GetPat();
            var connection = new VssConnection(new Uri(Ado.AdoUrl), new VssBasicCredential(string.Empty, pat));
            var witHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var workItem = await witHttpClient.GetWorkItemAsync(this.BugId);

            if (workItem.Fields.TryGetValue(fieldName, out object? htmlFieldContent))
            {
                var htmlContent = htmlFieldContent?.ToString();
                var withNewlines = Regex.Replace(htmlContent!, @"</div>", "\n", RegexOptions.IgnoreCase);
                withNewlines = Regex.Replace(withNewlines, @"</p>", "\n", RegexOptions.IgnoreCase);
                withNewlines = Regex.Replace(withNewlines, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
                var plainText = Regex.Replace(withNewlines, @"<[^>]+>", string.Empty);
                plainText = WebUtility.HtmlDecode(plainText);
                return plainText;
            }

            Console.WriteLine($"Field '{fieldName}' not found for work item ID {this.BugId}");
            return string.Empty;
        }

    }
}