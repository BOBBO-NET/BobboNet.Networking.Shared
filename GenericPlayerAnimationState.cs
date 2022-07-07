using LiteNetLib.Utils;

namespace BobboNet.Networking
{
    public abstract class GenericPlayerAnimationState<SelfType> : INetSerializable, ICopyConstructor<SelfType>
        where SelfType : GenericPlayerAnimationState<SelfType>
    {
        //
        //  Properties
        //

        public float VerticalLook { get; set; }
        public GenericGroundAnimationType GroundedType { get; set; }


        //
        //  Constructors
        //

        public GenericPlayerAnimationState() { }

        public SelfType Copy(SelfType other)
        {
            this.VerticalLook = other.VerticalLook;
            this.GroundedType = other.GroundedType;

            return (SelfType)this;
        }

        //
        //  Serialization
        //

        public void Deserialize(NetDataReader reader)
        {
            VerticalLook = reader.GetFloat();
            GroundedType = (GenericGroundAnimationType)reader.GetChar();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(VerticalLook);
            writer.Put((char)GroundedType);
        }
    }
}