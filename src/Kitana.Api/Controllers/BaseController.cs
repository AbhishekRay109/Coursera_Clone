using Kitana.Core.Logger;
using Kitana.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Data;

namespace Kitana.Api.Controllers
{
    /// <summary>
    /// This is a controller that is inherited by al the other controllers and it implements logger
    /// </summary>
    [Route("api/v1/")]
    public abstract class BaseController : Controller, IDisposable
    {
        /// <summary>
        /// Represents a protected instance of the CloudWatchLogger used for logging information to Amazon CloudWatch.
        /// </summary>
        /// <remarks>
        /// The CloudWatchLogger is a component for logging to Amazon CloudWatch.
        /// </remarks>
        protected CloudWatchLogger CWLogger;

        /// <summary>
        /// Represents a protected instance of the Serilog.Core.Logger used for logging purposes.
        /// </summary>
        /// <remarks>
        /// The Serilog.Core.Logger is a versatile logging tool that supports structured logging and various output sinks.
        /// </remarks>
        protected Serilog.Core.Logger logger;

        /// <summary>
        /// This is the constructor for base controller that takes logger as parameter
        /// </summary>
        protected BaseController(CloudWatchLogger CWLogger, Serilog.Core.Logger logger)
        {
            this.CWLogger = CWLogger ?? throw new ArgumentException(nameof(CWLogger));
            this.logger = logger ?? throw new ArgumentException(nameof(logger));
        }
        /// <summary>
        /// This is a protected function for logging any information
        /// </summary>
        /// // Common functionality, e.g., logging
        protected void LogInformation(string message, string streamName)
        {
            _ = CWLogger.log(message, streamName);
            logger.Information(message);
        }

        /// <summary>
        /// Logs an error message along with details of the associated exception.
        /// </summary>
        /// <param name="message">The error message to be logged.</param>
        /// <param name="exception">The exception associated with the error.</param>
        /// <remarks>
        /// This method is responsible for logging error information.
        /// </remarks>
        protected void LogError(string message, Exception exception)
        {
            _ = CWLogger.log(message);
            logger.Error(exception, message);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
        /// </summary>
        /// <remarks>
        /// This method is part of the IDisposable interface and is responsible for releasing and cleaning up resources
        /// used by the object.
        /// </remarks>
        protected new void Dispose()
        {
            if (logger is IDisposable disposableLogger)
            {
                disposableLogger.Dispose();
            }
        }
    }
}