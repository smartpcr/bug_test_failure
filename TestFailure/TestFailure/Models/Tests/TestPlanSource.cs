// -----------------------------------------------------------------------
// <copyright file="TestPlanSource.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    public class TestPlanSource
    {
        public string name { get; set; }
        public List<TestStage> testStages { get; set; }
    }
}