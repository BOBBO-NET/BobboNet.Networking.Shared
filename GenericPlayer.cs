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
            this.SetPositionAndVelocity(newUpdate.Position, newUpdate.Velocity);
            this.SetRotation(newUpdate.Rotation);
            this.SetAnimationState(newUpdate.Animation);
        }

        public override PlayerUpdate CommitToPlayerUpdate()
        {
            PlayerUpdate updateData = CreatePlayerUpdate();

            lastPlayerUpdate = new PlayerUpdate();
            lastPlayerUpdate.Copy(updateData);
            lastPlayerUpdateTime = DateTime.UtcNow;

            return updateData;
        }

        public override PlayerUpdate CreatePlayerUpdate()
        {
            return new PlayerUpdate()
            {
                Id = this.GetID(),
                Position = new NetVec3(this.Position),
                Velocity = new NetVec3(this.Velocity),
                Rotation = this.Rotation,
                Animation = new AnimationState().Copy(this.Animation)
            };
        }

        public override bool ShouldPlayerUpdate()
        {
            // If we have changed our footing (in air vs landed)
            if(this.Animation.GroundedType != lastPlayerUpdate.Animation.GroundedType) 
            {
                return true;
            }

            // If we've rotated enough to count as an update...
            if(System.Math.Abs(this.Rotation - lastPlayerUpdate.Rotation) > MinAngularDeltaUpdate)
            {
                return true;
            }

            // If we've rotated enough VERTICALLY to count as an update...
            if(System.Math.Abs(this.Animation.VerticalLook - lastPlayerUpdate.Animation.VerticalLook) > MinAngularDeltaUpdate)
            {
                return true;
            }

            //
            //  Extrapolate Positions
            //

            float oldDeltaTime = (float)((DateTime.UtcNow - lastPlayerUpdateTime).TotalSeconds);
            float currentDeltaTime = (float)((DateTime.UtcNow - lastPositionUpdateTime).TotalSeconds);

            // Where we THINK we've moved too since the last player update.
            NetVec3 oldExtrapolatedPos = lastPlayerUpdate.Position + (lastPlayerUpdate.Velocity * oldDeltaTime);

            // Where we have CURRENTLY moved to since last updating our position.
            NetVec3 currentExtrapolatedPos = this.Position + (this.Velocity * currentDeltaTime);

            // If the difference between our extrapolated position and our ACTUAL current position is too large...
            if(NetVec3.DeltaMagnitude(currentExtrapolatedPos, oldExtrapolatedPos) > MaxPositionError)
            {
                return true;
            }

            // OTHERWISE...
            // ...this player has not moved enough. Don't waste bandwidth sending an update.
            return false;
        }
    }
}