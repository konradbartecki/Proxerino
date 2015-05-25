namespace Proxerino.Socks_v5
{
    public class StageEnum
    {
        public enum Stage
        {
            ClientConnected = 0,
            ReceivedMethods = 1,
            SendBestMethod = 2,
            ReceivedRequest = 3,
            EstabilishingOutgoingConnection = 4,
            StreamingMode = 5,
            ExitNodeDisconnected = 6,
            ClientDisconnected = 7,
            Stopped = 8
        };
    }
}
