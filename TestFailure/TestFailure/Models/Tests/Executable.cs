// -----------------------------------------------------------------------
// <copyright file="Executable.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models.Tests
{
    public class Executable
    {
        public string path { get; set; }
        public bool outputXml { get; set; }
        public List<string> @params { get; set; }
        public bool regexExpansion { get; set; }
        public bool useNewSession { get; set; }
        public string errorAction { get; set; }
        public string outputXmlPrefix { get; set; }
        public bool validateExistance { get; set; }
    }
}