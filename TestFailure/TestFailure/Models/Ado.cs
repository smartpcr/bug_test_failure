// -----------------------------------------------------------------------
// <copyright file="Ado.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models
{
    public static class Ado
    {
        public const string Account = "msazure";
        public const string AdoUrl = "https://dev.azure.com/" + Ado.Account;
        public const string Project = "One";
        public const string ReproStepsField = "Microsoft.VSTS.TCM.ReproSteps";

        public static string GetPat()
        {
            var patFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".secrets\pat");
            var pat = File.ReadAllText(patFile).Trim();
            return pat;
        }
    }
}