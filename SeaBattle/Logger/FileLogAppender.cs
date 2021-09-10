using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SeaBattle.Logger
{
    class FileLogAppender : ILogAppender
    {
        private readonly string pathToFile;
        private readonly int fileSizeKb;
        private string fileTimeSuffix = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        private int counter;
        private FileStream stream;
        private StreamWriter writer;

        public FileLogAppender(string pathToFile = @"temp\logs\log_[time]_[counter].txt", int fileSizeKb = 30)
        {
            this.pathToFile = pathToFile;
            this.fileSizeKb = fileSizeKb;
            CreateFolderHierarchy();
        }

        public void HandleLogEvent(LogEvent logEvent)
        {
            if (stream is null || !stream.CanWrite)
            {
                stream = new FileStream(GetLogFileName(), FileMode.Append);
                writer = new StreamWriter(stream);
            }
            
            var exception = logEvent.Exception is null ? "" : "\n" +  logEvent.Exception;
            string logString = $"{logEvent.Time.ToString(CultureInfo.CurrentCulture)}.{logEvent.Time.Millisecond}\t" +
                               $"{logEvent.LogLevel}\t{logEvent.ThreadName}\t{logEvent.ThreadId}\t" +
                               $"{logEvent.ClassName}#{logEvent.MethodName}[{logEvent.Line}]\t" +
                               $"\t{logEvent.Message}{exception}";

            writer.WriteLine(logString);
            writer.Flush();

            if (stream.Position > fileSizeKb * 1024)
            {
                writer.Close();
                counter++;
            }
        }

        private void CreateFolderHierarchy()
        {
            var hierarchy = pathToFile.Split('\\');

            Directory.CreateDirectory(String.Join('\\', hierarchy.SkipLast(1)));
        }

        public void Dispose()
        {
            writer?.Dispose();
        }

        private string GetLogFileName()
        {
            string fileName = pathToFile
                .Replace("[time]", fileTimeSuffix)
                .Replace("[counter]", counter.ToString());
            return fileName;
        }
    }
}
