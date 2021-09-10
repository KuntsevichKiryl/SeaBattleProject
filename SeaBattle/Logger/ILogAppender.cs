using System;

namespace SeaBattle.Logger
{
    interface ILogAppender : IDisposable
    {
       void HandleLogEvent(LogEvent logEvent);
    }
}
