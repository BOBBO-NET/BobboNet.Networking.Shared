using LiteNetLib.Utils;
using System;

namespace BobboNet.Networking
{
    public abstract class GenericPlayer<AnimationState, UpdateDataType> : GamePlayer<UpdateDataType>
        where AnimationState : GenericPlayerAnimationState<AnimationState>, new()
        where UpdateDataType : GenericPlayerUpdateData<UpdateDataType, AnimationState>, new()
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

        public UpdateDataType CurrentData               { get; private set; } = new UpdateDataType();

        //
        //  Variables
        //

        private DateTime lastPositionUpdateTime =  DateTime.UtcNow;
        private UpdateDataType lastPlayerUpdate = new UpdateDataType();
        private DateTime lastPlayerUpdateTime = DateTime.UtcNow;

        //
        //  GamePlayer Methods
        //

        public override void ApplyPlayerUpdate(UpdateDataType newUpdate)
        {
            // If the update contains positional info, apply that & keep track of timing for extrapolation
            if((newUpdate.Type & GenericPlayerUpdateType.Position) == GenericPlayerUpdateType.Position)
            {
                CurrentData.Position = newUpdate.Position;

                // If the squared velocity is under the minimum, then set velocity to zero.
                if(newUpdate.Velocity.SqrMagnitude() < MinVelocityMagnitude) 
                {
                    CurrentData.Velocity = new NetVec3();
                }
                else 
                {
                    CurrentData.Velocity = newUpdate.Velocity;
                }
                // Cache the time that we updated position at for extrapolation
                this.lastPositionUpdateTime = DateTime.UtcNow;
            }

            // If the update contains rotational info, apply that!
            if((newUpdate.Type & GenericPlayerUpdateType.Rotation) == GenericPlayerUpdateType.Rotation)
            {
                CurrentData.RotationHorizontal = newUpdate.RotationHorizontal;
                CurrentData.RotationVertical = newUpdate.RotationVertical;
            }

            // If the update contains new animation info, apply that!
            if((newUpdate.Type & GenericPlayerUpdateType.Animation) == GenericPlayerUpdateType.Animation)
            {
                CurrentData.Animation.Copy(newUpdate.Animation);
            }            
        }

        public override UpdateDataType CommitToPlayerUpdate()
        {
            UpdateDataType updateData = CreatePlayerUpdate();
            updateData.Type = GetPlayerUpdateType();

            lastPlayerUpdate = new UpdateDataType();
            lastPlayerUpdate.Copy(updateData);
            lastPlayerUpdateTime = DateTime.UtcNow;

            // Apply extrapolated position locally!
            CurrentData.Position = updateData.Position;
            lastPositionUpdateTime = DateTime.UtcNow;

            return updateData;
        }

        public override UpdateDataType CreatePlayerUpdate()
        {
            UpdateDataType update = new UpdateDataType();
            update.Copy(CurrentData);
            update.Position = this.ExtrapolatePosition();
            
            return update;
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
            if(CurrentData.Animation.GroundedType != lastPlayerUpdate.Animation.GroundedType) 
            {
                result |= GenericPlayerUpdateType.Animation;
            }

            

            // If we've rotated enough HORIZONTALLY to count as an update...
            if(System.Math.Abs(CurrentData.RotationHorizontal - lastPlayerUpdate.RotationHorizontal) > MinAngularDeltaUpdate)
            {
                result |= GenericPlayerUpdateType.Rotation;
            }

            // If we've rotated enough VERTICALLY to count as an update...
            if(System.Math.Abs(CurrentData.RotationVertical - lastPlayerUpdate.RotationVertical) > MinAngularDeltaUpdate)
            {
                result |= GenericPlayerUpdateType.Rotation;
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
                result |= GenericPlayerUpdateType.Position;
            }

            // Return the combined effort of all above statements!
            return result;
        }

        private NetVec3 ExtrapolatePosition()
        {
            float currentDeltaTime = (float)((DateTime.UtcNow - lastPositionUpdateTime).TotalSeconds);
            return CurrentData.Position + (CurrentData.Velocity * currentDeltaTime);
        }
    }
}