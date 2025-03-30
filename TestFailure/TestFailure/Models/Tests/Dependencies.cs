// -----------------------------------------------------------------------
// <copyright file="Dependencies.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    using Newtonsoft.Json.Linq;

    public class Dependencies
    {
        public List<JObject> sources { get; set; }
        public List<JObject> targets { get; set; }
    }
}