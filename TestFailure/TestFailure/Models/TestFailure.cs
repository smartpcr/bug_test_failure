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
        public TestFailureReason Reason { get; set; }
        public bool IsTimeout { get; set; }

        public static TestFailure ParseTestFailure(string testCase, string testContent)
        {
            var testFailure = new TestFailure();
            testFailure.TestCase = testCase;

            var exMatch = RegexPatterns.exceptionRegex.Match(testContent);
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
                    if (testFailure.Error.IndexOf("establish remote session", StringComparison.Ordinal) >= 0)
                    {
                        testFailure.Reason = TestFailureReason.UnableEstablishRemoteSession;
                    }
                    else if (testFailure.Error.IndexOf("to not contain UpdateState.HealthCheckFailed", StringComparison.Ordinal) >= 0)
                    {
                        testFailure.Reason = TestFailureReason.HealthCheckFailed;
                    }
                    else if (testFailure.Error.IndexOf("failed to reach a terminal state", StringComparison.Ordinal) >= 0 &&
                             testFailure.Error.IndexOf("Current state: HealthChecking", StringComparison.Ordinal) > 0)
                    {
                        testFailure.Reason = TestFailureReason.HealthCheckFailed;
                    }
                    else if (testFailure.Error.IndexOf("GatewayTimeout was received", StringComparison.Ordinal) >= 0)
                    {
                        testFailure.Reason = TestFailureReason.GatewayTimeout;
                    }
                    else if (testFailure.Error.IndexOf("DownloadString", StringComparison.Ordinal) >= 0 &&
                             testFailure.Error.IndexOf("The remote server returned an error: (403) Forbidden", StringComparison.Ordinal) > 0)
                    {
                        testFailure.Reason = TestFailureReason.AuthorizationError;
                    }
                    else if (testFailure.Error.IndexOf("Action plan failed when trying to set override update configuration value", StringComparison.Ordinal) >= 0)
                    {
                        testFailure.Reason = TestFailureReason.EceError;
                    }
                }
            }

            return testFailure;
        }
    }

    public enum TestFailureReason
    {
        Unknown,
        UnableEstablishRemoteSession,
        HealthCheckFailed,
        GatewayTimeout,
        AuthorizationError,
        EceError
    }
}