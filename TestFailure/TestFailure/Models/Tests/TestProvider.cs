// -----------------------------------------------------------------------
// <copyright file="TestProvider.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    public class TestProvider
    {
        public string name { get; set; }
        public int timeoutInMinutes { get; set; }
        public List<Executable> synchronousExecutables { get; set; }
        public bool abortOnTimeout { get; set; }
        public List<string> waitOn { get; set; }
        public Dependencies dependencies { get; set; }
    }
}