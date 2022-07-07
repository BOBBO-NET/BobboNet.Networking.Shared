using LiteNetLib.Utils;

namespace BobboNet.Networking.Messages.Generic
{
    public abstract class SM_GenericBatchPlayerUpdates<PlayerUpdate> : INetSerializable 
        where PlayerUpdate : class, INetSerializable, new()
    {
        public PlayerUpdate[] Updates { get; set; } = new PlayerUpdate[0];

        //
        //  Constructors
        //

        public SM_GenericBatchPlayerUpdates() { }

        //
        //  Serialization
        //

        public void Deserialize(NetDataReader reader)
        {
            int arrayLength = reader.GetInt();
            Updates = new PlayerUpdate[arrayLength];
            for(int i = 0; i < Updates.Length; i++)
            {
                Updates[i] = new PlayerUpdate();
                Updates[i].Deserialize(reader);
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Updates.Length);
            for(int i = 0; i < Updates.Length; i++)
            {
                Updates[i].Serialize(writer);
            }
        }
    }
}