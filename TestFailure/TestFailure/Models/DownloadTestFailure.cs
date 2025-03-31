// -----------------------------------------------------------------------
// <copyright file="DownloadTestFailure.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models
{
    public class DownloadTestFailure
    {
        private const string ErrorSplitter = @"\n\n";
        private const string StackTraceSplitter = "--- End of stack trace from previous location where exception was thrown ---";

        public static void InferTestFailureReason(string logShareFolder, List<TestFailure> testFailures)
        {
            if (!Directory.Exists(logShareFolder))
            {
                Console.WriteLine($"Log share folder does not exist: {logShareFolder}");
                return;
            }

            var downloadTestResultFolder = logShareFolder;
            var testFolderName = Path.GetFileName(logShareFolder);
            if (testFolderName != "ASZ_BVT-Results")
            {
                downloadTestResultFolder = Path.Combine(logShareFolder, @"ASZ_BVT-Results\Download");
            }
            else
            {
                downloadTestResultFolder = Path.Combine(logShareFolder, @"Download");
            }

            if (!Directory.Exists(downloadTestResultFolder))
            {
                Console.WriteLine($"Download test result folder does not exist: {downloadTestResultFolder}");
                return;
            }

            foreach (var testFailure in testFailures)
            {
                if (testFailure.TestProvider == "Update")
                {
                    testFailure.Reason = UpdateTestFailure.InferTestFailureReason(testFailure.Error ?? "").ToString();
                }
                else if (testFailure.TestProvider == "Download")
                {
                    var failureReason = DownloadTestFailureReason.Unknown;
                    var testOutputFileName = $"Run-Download-{testFailure.TestCase}_console.txt";
                    var testOutputFilePath = Path.Combine(downloadTestResultFolder, testOutputFileName);
                    if (!File.Exists(testOutputFilePath))
                    {
                        Console.WriteLine($"Test output file does not exist: {testOutputFilePath}");
                        continue;
                    }

                    var testOutput = File.ReadAllText(testOutputFilePath);
                    if (testOutput.IndexOf(DownloadTestFailure.StackTraceSplitter, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        testOutput = testOutput.Substring(0, testOutput.IndexOf(DownloadTestFailure.StackTraceSplitter, StringComparison.OrdinalIgnoreCase));
                    }

                    var isStandalone = testOutput.Contains("download.exe", StringComparison.OrdinalIgnoreCase);

                    if (testOutput.IndexOf(ErrorSplitter, StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        testOutput = testOutput.Substring(testOutput.IndexOf(ErrorSplitter, StringComparison.OrdinalIgnoreCase) + ErrorSplitter.Length);
                    }

                    if (string.IsNullOrWhiteSpace(testFailure.Error))
                    {
                        testFailure.Error = testOutput;
                    }

                    if (testOutput.IndexOf("The operation has timed out", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        failureReason = isStandalone
                            ? DownloadTestFailureReason.DownloadStandaloneTimeout
                            : DownloadTestFailureReason.DownloadServiceTimeout;
                    }
                    else if (testOutput.IndexOf("UdiApi.DownloaderSession.Scan", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        failureReason = DownloadTestFailureReason.OsScanFailed;
                    }
                    else if (testOutput.IndexOf("no test results were found", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        failureReason = testFailure.IsTimeout ? DownloadTestFailureReason.Timeout : DownloadTestFailureReason.MissingTestOutput;
                    }

                    testFailure.Reason = failureReason.ToString();
                }
            }
        }
    }

    public enum DownloadTestFailureReason
    {
        Unknown,
        DownloadServiceTimeout,
        DownloadStandaloneTimeout,
        OsScanFailed,
        MissingTestOutput,
        Timeout
    }
}