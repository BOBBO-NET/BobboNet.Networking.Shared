using LiteNetLib.Utils;

namespace BobboNet.Networking.Messages.Generic
{
    public class SM_GenericInitialPlayerUpdates<UpdateMessageType, UpdateDataType, SM_BatchPlayerUpdates> : INetSerializable
        where UpdateMessageType : SCM_GenericPlayerUpdate<UpdateMessageType, UpdateDataType>, new()
        where UpdateDataType : class, INetSerializable, ICopyConstructor<UpdateDataType>, new()
        where SM_BatchPlayerUpdates : SM_GenericBatchPlayerUpdates<UpdateMessageType, UpdateDataType>, new()
    {
        public SM_BatchPlayerUpdates BatchUpdates { get; set; } = new SM_BatchPlayerUpdates();

        //
        //  Constructors
        //

        public SM_GenericInitialPlayerUpdates() { }

        //
        //  Serialization
        //

        public void Deserialize(NetDataReader reader)
        {
            BatchUpdates.Deserialize(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            BatchUpdates.Serialize(writer);
        }
    }
}