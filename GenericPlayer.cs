using LiteNetLib.Utils;
using System;

namespace BobboNet.Networking
{
    public abstract class GenericPlayer<PlayerUpdate, AnimationState> : GamePlayer<PlayerUpdate>
        where PlayerUpdate : GenericPlayerUpdate<PlayerUpdate, AnimationState>, new()
        where AnimationState : GenericPlayerAnimationState<AnimationState>, new()
    {
        //
        //  Protected Properties
        //

        // How much a player must rotate before an update is mandatory.
        protected virtual float MinAngularDeltaUpdate   { get { return 11.25f; } } 

        // The maximum positions should differ between the real player position and the extrapolated
        // player position.
        protected virtual float MaxPositionError        { get { return 0.1f * 0.1f; } } 

        // The minimum sqr velocity to be considered when setting velocity. Magnitudes below this
        // value will be ignored, and a velocity of (0, 0, 0) will be set instead. This is to reduce
        // Un-necessary jitter when starting & stopping player movement.         
        protected virtual float MinVelocityMagnitude    { get { return 1f * 1f; } } 

        //
        //  Properties
        //

        public NetVec3 Position                         { get; private set; } = new NetVec3();
        public NetVec3 Velocity                         { get; private set; } = new NetVec3();
        public float Rotation                           { get; private set; } = 0;
        public AnimationState Animation    { get; private set; } = new AnimationState();

        //
        //  Variables
        //

        private DateTime lastPositionUpdateTime =  DateTime.UtcNow;
        private PlayerUpdate lastPlayerUpdate = new PlayerUpdate();
        private DateTime lastPlayerUpdateTime = DateTime.UtcNow;


        //
        //  Public Methods
        //

        public void SetPositionAndVelocity(NetVec3 newPosition, NetVec3 newVelocity)
        {
            this.Position = newPosition;

            if(newVelocity.SqrMagnitude() < MinVelocityMagnitude) {
                this.Velocity = new NetVec3();
            }
            else {
                this.Velocity = newVelocity;
            }

            this.lastPositionUpdateTime = DateTime.UtcNow;
        }

        public void SetRotation(float newRotation)
        {
            this.Rotation = newRotation;
        }

        public void SetAnimationState(AnimationState newAnimation)
        {
            this.Animation = newAnimation;
        }

        //
        //  GamePlayer Methods
        //

        public override void ApplyPlayerUpdate(PlayerUpdate newUpdate)
        {
            if((newUpdate.Type & GenericPlayerUpdateType.Position) == GenericPlayerUpdateType.Position)
            {
                this.SetPositionAndVelocity(newUpdate.Position, newUpdate.Velocity);
            }

            if((newUpdate.Type & GenericPlayerUpdateType.Rotation) == GenericPlayerUpdateType.Rotation)
            {
                this.SetRotation(newUpdate.Rotation);
            }

            if((newUpdate.Type & GenericPlayerUpdateType.Animation) == GenericPlayerUpdateType.Animation)
            {
                this.SetAnimationState(newUpdate.Animation);
            }            
        }

        public override PlayerUpdate CommitToPlayerUpdate()
        {
            PlayerUpdate updateData = CreatePlayerUpdate();
            updateData.Type = GetPlayerUpdateType();

            lastPlayerUpdate = new PlayerUpdate();
            lastPlayerUpdate.Copy(updateData);
            lastPlayerUpdateTime = DateTime.UtcNow;

            // Apply extrapolated position locally!
            this.Position = updateData.Position;
            lastPositionUpdateTime = DateTime.UtcNow;

            return updateData;
        }

        public override PlayerUpdate CreatePlayerUpdate()
        {
            return new PlayerUpdate()
            {
                Id = this.GetID(),
                Type = GenericPlayerUpdateType.All,
                Position = new NetVec3(this.ExtrapolatePosition()),
                Velocity = new NetVec3(this.Velocity),
                Rotation = this.Rotation,
                Animation = new AnimationState().Copy(this.Animation)
            };
        }

        public override bool ShouldPlayerUpdate()
        {
            return GetPlayerUpdateType() != GenericPlayerUpdateType.None;
        }

        //
        //  Private Methods
        //

        private GenericPlayerUpdateType GetPlayerUpdateType()
        {
            GenericPlayerUpdateType result = GenericPlayerUpdateType.None;

            // If we have changed our footing (in air vs landed)
            if(this.Animation.GroundedType != lastPlayerUpdate.Animation.GroundedType) 
            {
                result &= GenericPlayerUpdateType.Animation;
            }

            // If we've rotated enough VERTICALLY to count as an update...
            if(System.Math.Abs(this.Animation.VerticalLook - lastPlayerUpdate.Animation.VerticalLook) > MinAngularDeltaUpdate)
            {
                result &= GenericPlayerUpdateType.Animation;
            }

            // If we've rotated enough to count as an update...
            if(System.Math.Abs(this.Rotation - lastPlayerUpdate.Rotation) > MinAngularDeltaUpdate)
            {
                result &= GenericPlayerUpdateType.Rotation;
            }


            //
            //  Extrapolate Positions
            //

            float oldDeltaTime = (float)((DateTime.UtcNow - lastPlayerUpdateTime).TotalSeconds);

            // Where we THINK we've moved too since the last player update.
            NetVec3 oldExtrapolatedPos = lastPlayerUpdate.Position + (lastPlayerUpdate.Velocity * oldDeltaTime);

            // Where we have CURRENTLY moved to since last updating our position.
            NetVec3 currentExtrapolatedPos = ExtrapolatePosition();

            // If the difference between our extrapolated position and our ACTUAL current position is too large...
            if(NetVec3.DeltaMagnitude(currentExtrapolatedPos, oldExtrapolatedPos) > MaxPositionError)
            {
                result &= GenericPlayerUpdateType.Position;
            }

            // Return the combined effort of all above statements!
            return result;
        }

        private NetVec3 ExtrapolatePosition()
        {
            float currentDeltaTime = (float)((DateTime.UtcNow - lastPositionUpdateTime).TotalSeconds);
            return this.Position + (this.Velocity * currentDeltaTime);
        }
    }
}