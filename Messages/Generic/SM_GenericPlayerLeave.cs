using LiteNetLib.Utils;

namespace BobboNet.Networking.Messages.Generic
{
    public class SM_GenericPlayerLeave<PlayerUpdate> : INetSerializable
        where PlayerUpdate : class, INetSerializable, new()
    {
        public int Id { get; set; }

        //
        //  Constructors
        //

        public SM_GenericPlayerLeave() { }

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