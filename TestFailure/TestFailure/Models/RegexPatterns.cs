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
        public static readonly Regex EnvironmentSplit = new Regex(
            @"(?=(?:Environment:\s+\w+\r?\nIP:\s+[\.\d]+|Environment Details:\s*\r?\n\s*FQDN:\s+[\w\.\-]+))",
            RegexOptions.Compiled
        );

        public static readonly Regex EnvironmentRegex = new Regex(@"Environment(?:\s+Name)?\s*:\s*(\w+)", RegexOptions.Compiled);

        public static readonly Regex IpRegex = new Regex(@"(?:DVM\s+or\s+Host\s+)?IP:\s+([\.\d]+)", RegexOptions.Compiled);

        public static readonly Regex LogShareRegex = new Regex(@"(?:SMB|Log)\s+Share:\s+([^\n]+)", RegexOptions.Compiled);

        public static readonly Regex PipelineLinkRegex = new Regex(@"(?:Pipeline Link|CI Job Output):\s+([^\n]+)", RegexOptions.Compiled);

        public static readonly Regex DateRegex = new Regex(@"(?:Retrieved\s+)?Date:\s+(\d{4}-\d{2}-\d{2}[T\s]\d{2}:\d{2}:\d{2})", RegexOptions.Compiled);

        public static readonly Regex BuildNumberRegex = new Regex(@"Build Number:\s+([^\s]+)", RegexOptions.Compiled);

        public static readonly Regex AeoDeviceUriRegex = new Regex(@"AEODeviceARMResourceUri:\s+([^\s]+)", RegexOptions.Compiled);

        public const string UpdateTestRegex = "RP: Update";
        public static string downloadTestRegex = "RP: Download"; // note: download failure are in log share
        public static readonly Regex TestSplit = new Regex(@"Test:\s+([^\s]+)", RegexOptions.Compiled);
        public static readonly Regex ExceptionRegex = new Regex(@"Exception:\s+([^\n]+)", RegexOptions.Compiled);
        public static readonly Regex BuildIdRegex = new Regex(@"buildId=(\d+)", RegexOptions.Compiled);
    }
}