// -----------------------------------------------------------------------
// <copyright file="TestResults.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    public class TestResults
    {
        // We map the three properties in the testResults object.
        public TestResultsSection AsynchronousTestProviders { get; set; }
        public TestResultsSection SynchronousTestProviders { get; set; }
        public TestResultsSection all { get; set; }
    }
}