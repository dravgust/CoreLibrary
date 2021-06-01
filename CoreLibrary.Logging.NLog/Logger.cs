using System;
using ILogger = CoreLibrary.Common.ILogger;

namespace CoreLibrary.Logging.NLog
{
    public class Logger : ILogger
    {
        private readonly global::NLog.Logger _logger;

        public Logger()
        {
            _logger = global::NLog.LogManager.GetCurrentClassLogger();
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }

        public void Error(Exception exception)
        {
            _logger.Error(exception);
        }

        public void Fatal(Exception exception)
        {
            _logger.Fatal(exception);
        }

        public void Error(object message)
        {
            _logger.Error(message);
        }

        public void ErrorFormat(string message, params object[] arguments)
        {
            _logger.Error(message, arguments);
        }

        public void DebugFormat(string message, params object[] arguments)
        {
            _logger.Debug(message, arguments);
        }

        public void Error(string unexpectedError, Exception exception)
        {
            _logger.Error(exception, unexpectedError);
        }
    }
}
