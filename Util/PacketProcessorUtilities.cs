using LiteNetLib.Utils;
using BobboNet.Networking.Messages;

namespace BobboNet.Networking.Util
{
    public static class PacketProcessorUtilities
    {
        // Register all nested types used for the base Bobbo-Net server and client
        public static void RegisterBobboNetNestedTypes(this NetPacketProcessor packetProcessor)
        {
            packetProcessor.RegisterNestedType<NetVec3>(() => new NetVec3());
        }
    }
}
