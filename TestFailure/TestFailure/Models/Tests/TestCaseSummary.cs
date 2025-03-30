// -----------------------------------------------------------------------
// <copyright file="TestCaseSummary.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    public class TestCaseSummary
    {
        public string Stage { get; set; }
        public string TestProvider { get; set; }
        public string Executable { get; set; }
        public bool NotRun { get; set; }
        public List<string> Exceptions { get; set; }
        public string Suite { get; set; }
        public string ResultFile { get; set; }
        public string TestCase { get; set; }
        public bool Succeeded { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool TimedOut { get; set; }
        public string Owner { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IncludeInReport { get; set; }
        // Optional additional properties:
        public string PSComputerName { get; set; }
        public string RunspaceId { get; set; }
        public bool? PSShowComputerName { get; set; }
    }
}