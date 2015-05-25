using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.UI.Xaml.Media;

namespace Proxerino
{
    public static class Config
    {
        public static class Listener
        {
            public const string DefaultServiceName = "1080";

            public static uint BufferSize = 64 * 1024;
            //You must enter your WinRT device IP address below
            public static HostName IP = new HostName("192.168.1.31");
            public static string ServiceName = "8080";
            public static SocketQualityOfService QoS = SocketQualityOfService.Normal;
            public static bool KeepConnectionsAlive = false;
            public static bool AllowOnlySocksV5Connections = false;
            public static bool UseCellularDataOnly = false;
            public static string WelcomeString = "Proxerino v{0} SOCKS server, hi!{1}Please specify your connection method{1} (in bytes)";

            //public static bool ListenOnAllInterfaces = true; not used
        }
        public static class About
        {
            public static string Version = "0.2";
            public static string Author = "Konrad Bartecki";
            public static string Email = "konradbartecki@outlook.com";
            public static string License = "";
        }

        public static class Logger
        {
            public static readonly string ServerStarting = "Server started at {0}:{1}"; //IP Address:Port
            public static readonly string ServerAlreadyRunning = "Server is already running";
            public static readonly string StoppingServer = "Stopping server";
            public static readonly string UsingPortOnly = "Using port only. Trying to determine IP Address...";

        }
    }
}
