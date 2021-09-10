namespace SeaBattle.Logger
{
    sealed class RootSBLogger : SBLogger
    {
        public RootSBLogger(LogLevel logLevel)
            : base("root", logLevel)
        {
        }
    }
}

