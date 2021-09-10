using System;

namespace SeaBattle.Logger
{
    class LogEvent
    {
        public DateTime Time { get; set; }
        public string LoggerName { get; set; }
        public LogLevel LogLevel { get; set; }
        public string ThreadName { get; set; }
        public int ThreadId { get; set; }
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public int Line { get; set; }
        public string MethodName { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
