using LiteNetLib.Utils;

namespace BobboNet.Networking
{
    public abstract class GenericPlayerUpdate<SelfType, AnimationState> : INetSerializable, ICopyConstructor<SelfType>
        where SelfType : GenericPlayerUpdate<SelfType, AnimationState>
        where AnimationState : GenericPlayerAnimationState<AnimationState>, new()
    {
        public int Id                           { get; set; }
        public GenericPlayerUpdateType Type     { get; set; }
        public NetVec3 Position                 { get; set; } = new NetVec3();
        public NetVec3 Velocity                 { get; set; } = new NetVec3();
        public float Rotation                   { get; set; }
        public AnimationState Animation         { get; set; } = new AnimationState();


        //
        //  Constructors
        //

        public GenericPlayerUpdate() { }

        public SelfType Copy(SelfType other)
        {
            this.Id = other.Id;
            this.Type = other.Type;
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
            this.Id = reader.GetInt();
            this.Type = (GenericPlayerUpdateType)reader.GetByte();

            // If we're storing position, then read it!
            if((this.Type & GenericPlayerUpdateType.Position) == GenericPlayerUpdateType.Position) 
            {
                Position.Deserialize(reader);
                Velocity.Deserialize(reader);
            }

            // If we're storing rotation, then read it!
            if((this.Type & GenericPlayerUpdateType.Rotation) == GenericPlayerUpdateType.Rotation) 
            {
                Rotation = reader.GetFloat();
            }

            // If we're storing animation data, then read it!
            if((this.Type & GenericPlayerUpdateType.Animation) == GenericPlayerUpdateType.Animation) 
            {
                Animation.Deserialize(reader);
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put((byte)Type);

            // If we're storing position, then write it!
            if((this.Type & GenericPlayerUpdateType.Position) == GenericPlayerUpdateType.Position) 
            {
                Position.Serialize(writer);
                Velocity.Serialize(writer);
            }

            // If we're storing rotation, then write it!
            if((this.Type & GenericPlayerUpdateType.Rotation) == GenericPlayerUpdateType.Rotation) 
            {
                writer.Put(Rotation);
            }

            // If we're storing animation data, then write it!
            if((this.Type & GenericPlayerUpdateType.Animation) == GenericPlayerUpdateType.Animation) 
            {
                Animation.Serialize(writer);
            }
        }
    }
}