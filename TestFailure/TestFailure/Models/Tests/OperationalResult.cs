// -----------------------------------------------------------------------
// <copyright file="OperationalResult.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    public class OperationalResult
    {
        public string Operation { get; set; }
        public string Status { get; set; }
        public object Exception { get; set; }
    }
}