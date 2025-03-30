// -----------------------------------------------------------------------
// <copyright file="TestResultsSection.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    public class TestResultsSection
    {
        public Quality quality { get; set; }
        public List<ConsolidatedSummary> consolidatedSummary { get; set; }
        public List<TestCaseSummary> testCaseSummary { get; set; }
    }
}