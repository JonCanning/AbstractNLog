using AbstractNLog.Interfaces;
using NLog;

namespace AbstractNLog
{
    public class Log : ILog
    {
        readonly Logger log;

        public Log(Logger log)
        {
            this.log = log;
        }

        public void InfoFormat(string format, params object[] args)
        {
            log.Info(format, args);
        }

        public void Error(object message)
        {
            log.Error(message);
        }

        public void Info(object message)
        {
            log.Info(message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            log.Warn(format, args);
        }
    }
}