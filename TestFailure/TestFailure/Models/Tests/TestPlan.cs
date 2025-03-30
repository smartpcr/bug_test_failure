// -----------------------------------------------------------------------
// <copyright file="TestPlan.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    public class TestPlan
    {
        public string testPlanName { get; set; }
        public TestPlanSource testPlanSource { get; set; }
        public string testPlanPath { get; set; }
        public List<OperationalResult> operationalResults { get; set; }
        public string reportHTMLPath { get; set; }
        public TestResults testResults { get; set; }
    }
}