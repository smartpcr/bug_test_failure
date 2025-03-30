// -----------------------------------------------------------------------
// <copyright file="TestStage.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    public class TestStage
    {
        public string name { get; set; }
        public string executionMode { get; set; }
        public List<TestProvider> testProviders { get; set; }
        public bool terminatesPlan { get; set; }
    }
}