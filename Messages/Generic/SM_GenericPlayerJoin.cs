using LiteNetLib.Utils;

namespace BobboNet.Networking.Messages.Generic
{
    public class SM_GenericPlayerJoin<PlayerUpdate> : INetSerializable
        where PlayerUpdate : class, INetSerializable, new()
    {
        public int Id { get; set; }

        //
        //  Constructors
        //

        public SM_GenericPlayerJoin() { }

        //
        //  Serialization
        //

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
        }
    }
}