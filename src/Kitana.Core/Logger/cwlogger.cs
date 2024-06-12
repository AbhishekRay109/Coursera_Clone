using Amazon.CloudWatchLogs.Model;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Amazon;

namespace Kitana.Core.Logger
{
    public class CloudWatchLogger
    {
        private readonly IAmazonCloudWatchLogs _cloudWatchLogsClient;
        private readonly string logGroupName1;
        private readonly string logGroupName2;
        private readonly string logStreamName;

        public CloudWatchLogger(IConfiguration configuration)
        {
            logGroupName1 = Environment.GetEnvironmentVariable("LogGroupName1");
            logGroupName2 = Environment.GetEnvironmentVariable("LogGroupName2");
            logStreamName = Environment.GetEnvironmentVariable("LogStreamName");
            var accessKey = Environment.GetEnvironmentVariable("AccessKey");
            var secretKey = Environment.GetEnvironmentVariable("SecretKey");
            var region = Environment.GetEnvironmentVariable("Region");

            var awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            var awsRegion = RegionEndpoint.GetBySystemName(region);
            _cloudWatchLogsClient = new AmazonCloudWatchLogsClient(awsCredentials, awsRegion);
        }

        public async Task log(string message, string streamName = null)
        {
            try
            {
                var logStreamNames = streamName ?? this.logStreamName;
                var logGroupNameToUse = streamName != null ? logGroupName2 : logGroupName1;

                var describeGroupRequest = new DescribeLogGroupsRequest
                {
                    LogGroupNamePrefix = logGroupNameToUse
                };

                var describeGroupResponse = await _cloudWatchLogsClient.DescribeLogGroupsAsync(describeGroupRequest);

                if (describeGroupResponse.LogGroups.Count == 0)
                {
                    var groupRequest = new CreateLogGroupRequest
                    {
                        LogGroupName = logGroupNameToUse
                    };

                    await _cloudWatchLogsClient.CreateLogGroupAsync(groupRequest);
                }

                var describeStreamRequest = new DescribeLogStreamsRequest
                {
                    LogGroupName = logGroupNameToUse,
                    LogStreamNamePrefix = logStreamNames
                };

                var describeStreamResponse = await _cloudWatchLogsClient.DescribeLogStreamsAsync(describeStreamRequest);

                if (describeStreamResponse.LogStreams.Count == 0)
                {
                    var streamRequest = new CreateLogStreamRequest
                    {
                        LogGroupName = logGroupNameToUse,
                        LogStreamName = logStreamNames
                    };

                    await _cloudWatchLogsClient.CreateLogStreamAsync(streamRequest);
                }
                var request = new PutLogEventsRequest
                {
                    LogGroupName = logGroupNameToUse,
                    LogStreamName = logStreamNames,
                    LogEvents = new List<InputLogEvent>
            {
                new InputLogEvent
                {
                    Message = message,
                    Timestamp = DateTime.UtcNow
                }
            }
                };

                await _cloudWatchLogsClient.PutLogEventsAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong!: {ex.Message}");
                throw;
            }
        }

    }
}
