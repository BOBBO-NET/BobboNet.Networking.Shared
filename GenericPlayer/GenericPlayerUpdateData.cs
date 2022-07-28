using LiteNetLib.Utils;

namespace BobboNet.Networking
{
    public abstract class GenericPlayerUpdateData<SelfType, AnimationState> : INetSerializable, ICopyConstructor<SelfType>
        where SelfType : GenericPlayerUpdateData<SelfType, AnimationState>
        where AnimationState : GenericPlayerAnimationState<AnimationState>, new()
    {
        public GenericPlayerUpdateType Type     { get; set; } = GenericPlayerUpdateType.None;
        public NetVec3 Position                 { get; set; } = new NetVec3();
        public NetVec3 Velocity                 { get; set; } = new NetVec3();
        public float RotationHorizontal         { get; set; }
        public float RotationVertical           { get; set; }
        public AnimationState Animation         { get; set; } = new AnimationState();


        //
        //  Constructors
        //

        public GenericPlayerUpdateData() { }

        public virtual SelfType Copy(SelfType other)
        {
            this.Type = other.Type;
            this.Position = new NetVec3(other.Position);
            this.Velocity = new NetVec3(other.Velocity);
            this.RotationHorizontal = other.RotationHorizontal;
            this.RotationVertical = other.RotationVertical;
            this.Animation.Copy(other.Animation);

            return (SelfType)this;
        }

        //
        //  Public Methods
        //

        public SelfType ApplyPosition(NetVec3 position, NetVec3 velocity)
        {
            this.Type |= GenericPlayerUpdateType.Position;
            this.Position = position;
            this.Velocity = velocity;

            return (SelfType)this;
        }

        public SelfType ApplyRotation(float rotationHorizontal, float rotationVertical)
        {
            this.Type |= GenericPlayerUpdateType.Rotation;
            this.RotationHorizontal = RotationHorizontal;
            this.RotationVertical = RotationVertical;

            return (SelfType)this;
        }

        public SelfType ApplyAnimationState(AnimationState animationState)
        {
            this.Type |= GenericPlayerUpdateType.Animation;
            this.Animation.Copy(animationState);

            return (SelfType)this;
        }

        //
        //  Serialization
        //

        public void Deserialize(NetDataReader reader)
        {
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
                RotationHorizontal = reader.GetFloat();
                RotationVertical = reader.GetFloat();
            }

            // If we're storing animation data, then read it!
            if((this.Type & GenericPlayerUpdateType.Animation) == GenericPlayerUpdateType.Animation) 
            {
                Animation.Deserialize(reader);
            }
        }

        public void Serialize(NetDataWriter writer)
        {
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
                writer.Put(RotationHorizontal);
                writer.Put(RotationVertical);
            }

            // If we're storing animation data, then write it!
            if((this.Type & GenericPlayerUpdateType.Animation) == GenericPlayerUpdateType.Animation) 
            {
                Animation.Serialize(writer);
            }
        }
    }
}