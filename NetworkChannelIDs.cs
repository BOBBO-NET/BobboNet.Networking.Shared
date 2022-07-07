using LiteNetLib.Utils;

namespace BobboNet.Networking
{
    public static class NetworkChannelIDs
    {
        //
        //  Constant Settings
        //

        public const byte ChannelCount      = 2;
        public const byte Unreliable        = 0x00;
        public const byte ReliableOrdered   = 0x01;
    }
}
