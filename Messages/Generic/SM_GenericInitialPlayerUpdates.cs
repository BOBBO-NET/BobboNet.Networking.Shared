using LiteNetLib.Utils;

namespace BobboNet.Networking.Messages.Generic
{
    public class SM_GenericInitialPlayerUpdates<PlayerUpdate, SM_BatchPlayerUpdates> : INetSerializable
        where PlayerUpdate : class, INetSerializable, new()
        where SM_BatchPlayerUpdates : SM_GenericBatchPlayerUpdates<PlayerUpdate>, new()
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