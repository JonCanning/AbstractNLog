
namespace AbstractNLog
{
    public interface ILog
    {
        void InfoFormat(string format, params object[] args);
        void Error(object message);
        void Info(object message);
        void WarnFormat(string format, params object[] args);
    }
}