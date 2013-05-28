using NLog;
using NLog.Config;
using NLog.Layouts;
using System;
using System.Diagnostics;

namespace AbstractNLog
{
    public static class LogProvider
    {
        static LogProvider()
        {
            LogManager.Configuration = new LoggingConfiguration();
        }

        public static void AddSyslogEndpoint(string server, int port)
        {
            var syslog = new Syslog(server, port) { Facility = Syslog.SyslogFacility.Local0, Layout = new SimpleLayout("${message}") };
            var loggingRule = new LoggingRule("*", LogLevel.Debug, syslog);
            LogManager.Configuration.LoggingRules.Add(loggingRule);
            LogManager.Configuration.AddTarget(server, syslog);
        }

        public static ILog GetLogger<T>(this T obj)
        {
            var type = typeof(T);
            return GetLogger(type);
        }

        public static ILog GetLogger(Type type)
        {
            var log = LogManager.GetLogger(type.Name);
            return new Log(log);
        }

        public static ILog GetLogger()
        {
            var stackTrace = new StackFrame(1);
            var type = stackTrace.GetMethod().DeclaringType;
            return GetLogger(type);
        }
    }
}