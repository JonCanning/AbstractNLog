using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;

namespace AbstractNLog
{
    [Target("Syslog")]
    public class Syslog : TargetWithLayout
    {
        public enum SyslogFacility
        {
            Kernel,
            User,
            Mail,
            Daemons,
            Authorization,
            Syslog,
            Printer,
            News,
            Uucp,
            Clock,
            Authorization2,
            Ftp,
            Ntp,
            Audit,
            Alert,
            Clock2,
            Local0,
            Local1,
            Local2,
            Local3,
            Local4,
            Local5,
            Local6,
            Local7,
        }

        enum SyslogSeverity
        {
            Emergency,
            Alert,
            Critical,
            Error,
            Warning,
            Notice,
            Informational,
            Debug,
        }

        readonly string hostName;
        readonly IPAddress syslogServerIp;
        readonly UdpClient udpClient;
        readonly AutoResetEvent autoResetEvent = new AutoResetEvent(true);

        public Syslog(string syslogServer, int port)
        {
            syslogServerIp = Dns.GetHostAddresses(syslogServer).FirstOrDefault();
            if (syslogServerIp == null)
                throw new NullReferenceException(syslogServer);
            hostName = Dns.GetHostName();
            udpClient = new UdpClient(syslogServer, port);
        }

        public SyslogFacility Facility { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            var message = BuildSyslogMessage(Facility, GetSyslogSeverity(logEvent.Level), logEvent.LoggerName, Layout.Render(logEvent));
            Task.Factory.StartNew(() => Send(message));
        }

        void Send(byte[] message)
        {
            autoResetEvent.WaitOne();
            autoResetEvent.Reset();
            udpClient.Send(message, message.Length);
            autoResetEvent.Set();
        }

        static SyslogSeverity GetSyslogSeverity(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Fatal)
                return SyslogSeverity.Emergency;
            if (logLevel >= LogLevel.Error)
                return SyslogSeverity.Error;
            if (logLevel >= LogLevel.Warn)
                return SyslogSeverity.Warning;
            if (logLevel >= LogLevel.Info)
                return SyslogSeverity.Informational;
            if (logLevel >= LogLevel.Debug)
                return SyslogSeverity.Debug;
            return SyslogSeverity.Notice;
        }

        byte[] BuildSyslogMessage(SyslogFacility facility, SyslogSeverity priority, string sender, string body)
        {
            var message = string.Format("<{0}>{1} {2} {3}: {4}", (int) facility*8 + priority, DateTime.UtcNow.ToString("s"), hostName, sender, body);
            return Encoding.ASCII.GetBytes(message);
        }
    }
}