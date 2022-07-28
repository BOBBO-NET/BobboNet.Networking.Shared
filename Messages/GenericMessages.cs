using LiteNetLib.Utils;
using BobboNet.Networking.Messages.Generic;

namespace BobboNet.Networking.Messages
{
    public static class StandardMessages<UpdateDataType> where UpdateDataType : class, INetSerializable, ICopyConstructor<UpdateDataType>, new()
    {
        public class SCM_PlayerUpdate           : SCM_GenericPlayerUpdate<SCM_PlayerUpdate, UpdateDataType> { }
        public class SM_BatchPlayerUpdates      : SM_GenericBatchPlayerUpdates<SCM_PlayerUpdate, UpdateDataType> { }
        public class SM_InitialPlayerUpdates    : SM_GenericInitialPlayerUpdates<SCM_PlayerUpdate, UpdateDataType, SM_BatchPlayerUpdates> { }
        public class SM_PlayerJoin              : SM_GenericPlayerJoin<UpdateDataType> { }
        public class SM_PlayerLeave             : SM_GenericPlayerLeave<UpdateDataType> { }
    }
}