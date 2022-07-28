using LiteNetLib.Utils;

namespace BobboNet.Networking.Messages.Generic
{
    public abstract class SM_GenericBatchPlayerUpdates<UpdateMessageType, UpdateDataType> : INetSerializable 
        where UpdateMessageType : SCM_GenericPlayerUpdate<UpdateMessageType, UpdateDataType>, new()
        where UpdateDataType : class, INetSerializable, ICopyConstructor<UpdateDataType>, new()
    {
        public UpdateMessageType[] Updates { get; set; } = new UpdateMessageType[0];

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
            Updates = new UpdateMessageType[arrayLength];
            for(int i = 0; i < Updates.Length; i++)
            {
                Updates[i] = new UpdateMessageType();
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