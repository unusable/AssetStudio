using System;
using AssetStudio;

namespace AssetStudioConsole
{
    public class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {
        }

        public void Log(LoggerEvent loggerEvent, string message)
        {
            Console.WriteLine(loggerEvent.ToString() + " -- " + message);
        }
    }
}
