using LiteNetLib.Utils;

namespace BobboNet.Networking
{
    public abstract class GenericPlayerUpdate<SelfType, AnimationState> : INetSerializable, ICopyConstructor<SelfType>
        where SelfType : GenericPlayerUpdate<SelfType, AnimationState>
        where AnimationState : GenericPlayerAnimationState<AnimationState>, new()
    {
        public int Id                   { get; set; }
        public NetVec3 Position         { get; set; } = new NetVec3();
        public NetVec3 Velocity         { get; set; } = new NetVec3();
        public float Rotation           { get; set; }
        public AnimationState Animation { get; set; } = new AnimationState();


        //
        //  Constructors
        //

        public GenericPlayerUpdate() { }

        public SelfType Copy(SelfType other)
        {
            this.Id = other.Id;
            this.Position = new NetVec3(other.Position);
            this.Velocity = new NetVec3(other.Velocity);
            this.Rotation = other.Rotation;
            this.Animation.Copy(other.Animation);

            return (SelfType)this;
        }


        //
        //  Serialization
        //

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetInt();
            Position.Deserialize(reader);
            Rotation = reader.GetFloat();
            Animation.Deserialize(reader);
            Velocity.Deserialize(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            Position.Serialize(writer);
            writer.Put(Rotation);
            Animation.Serialize(writer);
            Velocity.Serialize(writer);
        }
    }
}