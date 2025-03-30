// -----------------------------------------------------------------------
// <copyright file="TestPlanRoot.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    public class TestPlanRoot
    {
        // Because the JSON root is an array, we assume:
        // index 0: a string with extra modules info,
        // index 1: a file path string,
        // index 2: the actual test plan object.
        public string ExtraModules { get; set; }
        public string? MASCIHelpersPath { get; set; }
        public TestPlan TestPlan { get; set; }
    }
}