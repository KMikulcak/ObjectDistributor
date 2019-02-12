using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ObjectDistributor.Core.Infrastructure
{
    internal class Bootstrapper
    {
        public static void Init()
        {
            var config = new LoggingConfiguration();

            var logfile = new FileTarget("logfile") {FileName = $"logfile_{DateTime.Now}.txt"};
            var logConsole = new ConsoleTarget {Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}"};

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
        }
    }
}