﻿using Mcma.Utility;

namespace Mcma.Logging.Aws.CloudWatch;

public static class McmaCloudWatchEnvironmentVariables
{
    public static string LogGroupName => McmaEnvironmentVariables.Get("LOG_GROUP_NAME");
}