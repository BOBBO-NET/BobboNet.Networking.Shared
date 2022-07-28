using LiteNetLib.Utils;

namespace BobboNet.Networking.Messages.Generic
{
    public abstract class SCM_GenericPlayerUpdate<SelfType, UpdateDataType> : INetSerializable, ICopyConstructor<SelfType>
        where SelfType : SCM_GenericPlayerUpdate<SelfType, UpdateDataType>
        where UpdateDataType : class, INetSerializable, ICopyConstructor<UpdateDataType>, new()
    {
        public int Id                   { get; set; }
        public UpdateDataType Data      { get; set; } = new UpdateDataType();

        //
        //  Constructors
        //

        public SCM_GenericPlayerUpdate() { }

        public SelfType Copy(SelfType other)
        {
            this.Id = other.Id;
            this.Data.Copy(other.Data);

            return (SelfType)this;
        }


        //
        //  Serialization
        //

        public void Deserialize(NetDataReader reader)
        {
            this.Id = reader.GetInt();
            this.Data.Deserialize(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            this.Data.Serialize(writer);
        }
    }
}