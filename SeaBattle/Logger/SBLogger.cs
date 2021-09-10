using System;
using System.Diagnostics;
using System.Threading;

namespace SeaBattle.Logger
{

    class SBLogger
    {
        public string Name { get; }
        public LogLevel Level { get; }

        public delegate void LogEventAppender(LogEvent logEvent);
        private event LogEventAppender Appender;

        public SBLogger(string name, LogLevel logLevel)
        {
            Name = name;
            Level = logLevel;
        }

        public SBLogger AddAppender(ILogAppender logAppender)
        {
            Appender += logAppender.HandleLogEvent;
            return this;
        }

        private void PublishLogEvent(LogLevel logLevel, string message, Exception exception)
        {
            if (logLevel > Level)
            {
                return;
            }

            var stackTrace = new StackTrace(2, true);
            var stackFrame = stackTrace.GetFrame(0);
            var thread = Thread.CurrentThread;
            var methodBase = stackFrame?.GetMethod();

            LogEvent logEvent = new LogEvent()
            {
                LoggerName = Name,
                LogLevel = Level,
                ThreadName = thread.Name,
                ThreadId = thread.ManagedThreadId,
                ClassName = methodBase?.DeclaringType?.FullName,
                Namespace = methodBase?.DeclaringType?.Namespace,
                Line = stackFrame?.GetFileLineNumber() ?? -1,
                MethodName = methodBase?.Name,
                Exception = exception,
                Message = message,
                Time = DateTime.Now
            };

            Appender?.Invoke(logEvent);
        }

        public void Info(string message, Exception exception = null)
        {
            PublishLogEvent(LogLevel.Info, message, exception);
        }

        public void Debug(string message, Exception exception = null)
        {
            PublishLogEvent(LogLevel.Debug, message, exception);
        }

        public void Error(string message, Exception exception = null)
        {
            PublishLogEvent(LogLevel.Error, message, exception);
        }
    }
}
