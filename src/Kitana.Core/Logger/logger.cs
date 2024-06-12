using Amazon.Runtime;
using Serilog;
using Serilog.Sinks.AwsCloudWatch;
using Amazon.CloudWatchLogs;
using Amazon;
using Serilog.Formatting.Compact;
using Microsoft.Extensions.Configuration;

namespace BaseCodeSetup.Core.logger
{
    public static class Logger
    {
        public static Serilog.Core.Logger CreateLogger()
        {
            var logPath = Environment.GetEnvironmentVariable("logPath");
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(new CompactJsonFormatter(), logPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30, fileSizeLimitBytes: 10 * 1024 * 1024)
                .CreateLogger();

            return logger;

        }
    }
}
