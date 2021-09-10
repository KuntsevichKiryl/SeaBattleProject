namespace SeaBattle.Logger
{
    class SBLoggerFactory
    {
        private static readonly ILogAppender appender = new FileLogAppender();
        private static readonly SBLogger root = new RootSBLogger(LogLevel.Info).AddAppender(appender);

        public static SBLogger GetLogger<T>(string name = null, LogLevel level = LogLevel.Root)
        {
            var loggerName = name ?? nameof(T);
            var loggerLevel = level == LogLevel.Root ? root.Level : level;
            return new SBLogger(loggerName, loggerLevel).AddAppender(appender);
        }
    }
}
