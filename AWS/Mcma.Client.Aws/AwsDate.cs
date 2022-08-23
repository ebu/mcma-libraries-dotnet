
using System;

namespace Mcma.Client.Aws;

public class AwsDate
{
    private DateTimeOffset UtcNow { get; } = DateTimeOffset.UtcNow;

    public string DateString => UtcNow.ToString("yyyyMMdd");

    public string DateTimeString => UtcNow.ToString("yyyyMMddTHHmmssZ");
}