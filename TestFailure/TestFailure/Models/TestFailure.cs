// -----------------------------------------------------------------------
// <copyright file="TestFailure.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models
{
    public class TestFailure
    {
        public string TestProvider { get; set; }
        public string TestCase { get; set; }
        public string ExceptionType { get; set; }
        public string? Error { get; set; }
        public string Reason { get; set; }
        public bool IsTimeout { get; set; }

        public static TestFailure ParseTestFailure(string testProvider, string testCase, string testContent)
        {
            var testFailure = new TestFailure();
            testFailure.TestProvider = testProvider;
            testFailure.TestCase = testCase;

            var exMatch = RegexPatterns.ExceptionRegex.Match(testContent);
            if (exMatch.Success)
            {
                var exceptionMessage = exMatch.Groups[1].Value;
                var nestedSplitIndex = exceptionMessage.IndexOf("--->", StringComparison.Ordinal);
                if (nestedSplitIndex > 0)
                {
                    exceptionMessage = exceptionMessage.Substring(0, nestedSplitIndex);
                }
                var exTypeIndex = exceptionMessage.IndexOf(":", StringComparison.Ordinal);
                if (exTypeIndex > 0)
                {
                    testFailure.ExceptionType = exceptionMessage.Substring(0, exTypeIndex).Trim();
                    testFailure.Error = exceptionMessage.Substring(exTypeIndex + 1).Trim();
                    testFailure.Reason = UpdateTestFailure.InferTestFailureReason(testFailure.Error).ToString();
                }
            }

            return testFailure;
        }
    }
}