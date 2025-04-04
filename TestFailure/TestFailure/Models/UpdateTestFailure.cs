// -----------------------------------------------------------------------
// <copyright file="UpdateTestFailure.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BugAnalysis.Models
{
    public class UpdateTestFailure
    {
        public static UpdateTestFailureReason InferTestFailureReason(string errorMessage)
        {
            if (errorMessage.IndexOf("establish remote session", StringComparison.Ordinal) >= 0)
            {
                return UpdateTestFailureReason.UnableEstablishRemoteSession;
            }

            if (errorMessage.IndexOf("to not contain UpdateState.HealthCheckFailed", StringComparison.Ordinal) >= 0)
            {
                return UpdateTestFailureReason.HealthCheckFailed;
            }

            if (errorMessage.IndexOf("failed to reach a terminal state", StringComparison.Ordinal) >= 0 &&
                errorMessage.IndexOf("Current state: HealthChecking", StringComparison.Ordinal) > 0)
            {
                return UpdateTestFailureReason.HealthCheckFailed;
            }

            if (errorMessage.IndexOf("GatewayTimeout was received", StringComparison.Ordinal) >= 0)
            {
                return UpdateTestFailureReason.GatewayTimeout;
            }

            if (errorMessage.IndexOf("DownloadString", StringComparison.Ordinal) >= 0 &&
                errorMessage.IndexOf("The remote server returned an error: (403) Forbidden", StringComparison.Ordinal) > 0)
            {
                return UpdateTestFailureReason.AuthorizationError;
            }

            if (errorMessage.IndexOf("Action plan failed when trying to set override update configuration value", StringComparison.Ordinal) >= 0)
            {
                return UpdateTestFailureReason.FailedUpdateConfigValueInActionPlan;
            }

            if (errorMessage.IndexOf("WebException.Status: ConnectFailure", StringComparison.Ordinal) >= 0 &&
                errorMessage.IndexOf("providers/Microsoft.Update.Admin/updateLocations", StringComparison.Ordinal) >= 0)
            {
                return UpdateTestFailureReason.FailedToGetUpdateLocations;
            }

            if (errorMessage.IndexOf("WebException.Status: ConnectFailure", StringComparison.Ordinal) >= 0 &&
                errorMessage.IndexOf(":4900/featureFlag", StringComparison.Ordinal) >= 0)
            {
                return UpdateTestFailureReason.FailedToGetFeatureFlags;
            }

            if (errorMessage.IndexOf("Unable to connect to the remote server", StringComparison.Ordinal) >= 0)
            {
                return UpdateTestFailureReason.FailedToConnectToUpdateService;
            }

            return UpdateTestFailureReason.Unknown;
        }
    }

    public enum UpdateTestFailureReason
    {
        Unknown,
        UnableEstablishRemoteSession,
        HealthCheckFailed,
        GatewayTimeout,
        AuthorizationError,
        EceError,
        FailedToConnectToUpdateService,
        FailedToGetUpdateLocations,
        FailedToGetFeatureFlags,
        FailedUpdateConfigValueInActionPlan
    }
}