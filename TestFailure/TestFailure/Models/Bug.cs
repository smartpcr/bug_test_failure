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

        public async Task<string> DownloadReproSteps()
        {
            var pat = Ado.GetPat();
            var connection = new VssConnection(new Uri(Ado.AdoUrl), new VssBasicCredential(string.Empty, pat));
            var witHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            var workItem = await witHttpClient.GetWorkItemAsync(this.BugId);
            if (workItem.Fields.TryGetValue(Ado.ReproStepsField, out object? reproSteps))
            {
                var htmlContent = reproSteps.ToString();
                string withNewlines = Regex.Replace(htmlContent!, @"</div>", "\n");
                withNewlines = Regex.Replace(withNewlines, @"</p>", "\n");
                withNewlines = Regex.Replace(withNewlines, @"<br>", "\n");
                string plainText = Regex.Replace(withNewlines, @"<[^>]+>", string.Empty);
                plainText = WebUtility.HtmlDecode(plainText);
                return plainText;
            }

            Console.WriteLine("Bug Repro Steps field not found for work item ID " + this.BugId);
            return string.Empty;
        }
    }
}