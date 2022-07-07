using LiteNetLib.Utils;
using BobboNet.Networking.Messages.Generic;

namespace BobboNet.Networking.Messages
{
    public static class StandardMessages<PlayerUpdate> where PlayerUpdate : class, INetSerializable, new()
    {
        public class SM_BatchPlayerUpdates      : SM_GenericBatchPlayerUpdates<PlayerUpdate> { }
        public class SM_InitialPlayerUpdates    : SM_GenericInitialPlayerUpdates<PlayerUpdate, SM_BatchPlayerUpdates> { }
        public class SM_PlayerJoin              : SM_GenericPlayerJoin<PlayerUpdate> { }
        public class SM_PlayerLeave             : SM_GenericPlayerLeave<PlayerUpdate> { }
    }
}