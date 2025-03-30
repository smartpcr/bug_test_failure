// -----------------------------------------------------------------------
// <copyright file="RegexPatterns.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models
{
    using System.Text.RegularExpressions;

    public static class RegexPatterns
    {
        public static Regex environmentSplit = new Regex(@"(?=Environment:\s+\w+\r?\nIP:\s+[\.\d]+)", RegexOptions.Compiled);
        public static Regex environmentRegex = new Regex(@"Environment:\s+(\w+)", RegexOptions.Compiled);
        public static Regex ipRegex = new Regex(@"IP:\s+([\.\d]+)", RegexOptions.Compiled);
        public static Regex logShareRegex = new Regex(@"Log Share:\s+([^\n]+)", RegexOptions.Compiled);
        public static Regex pipelineLinkRegex = new Regex(@"Pipeline Link:\s+([^\n]+)", RegexOptions.Compiled);
        public static Regex dateRegex = new Regex(@"Date:\s+(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})", RegexOptions.Compiled);
        public static Regex buildNumberRegex = new Regex(@"Build Number:\s+([^\s]+)", RegexOptions.Compiled);
        public static Regex aeoDeviceUriRegex = new Regex(@"AEODeviceARMResourceUri:\s+([^\s]+)", RegexOptions.Compiled);
        public static string updateTestRegex = "RP: Update";
        public static Regex testSplit = new Regex(@"Test:\s+([^\s]+)", RegexOptions.Compiled);
        public static Regex exceptionRegex = new Regex(@"Exception:\s+([^\n]+)", RegexOptions.Compiled);
        public static Regex buildIdRegex = new Regex(@"buildId=(\d+)", RegexOptions.Compiled);
    }
}