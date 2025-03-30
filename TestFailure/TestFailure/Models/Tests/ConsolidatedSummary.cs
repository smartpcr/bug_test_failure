// -----------------------------------------------------------------------
// <copyright file="ConsolidatedSummary.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    public class ConsolidatedSummary
    {
        public string Stage { get; set; }
        public string TestProvider { get; set; }
        public string Suite { get; set; }
        public string Executable { get; set; }
        public string ResultFile { get; set; }
        public string StartTimeUTC { get; set; }
        public string EndTimeUTC { get; set; }
        public int TotalTestCases { get; set; }
        public int SuccessfulTestCases { get; set; }
        public bool ProviderTimeout { get; set; }
        public double Quality { get; set; }
        public bool IncludeInReport { get; set; }
    }
}