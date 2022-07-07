using LiteNetLib;
using LiteNetLib.Utils;

namespace BobboNet.Networking
{
    public abstract class GamePlayer<PlayerUpdate>
        where PlayerUpdate : class, INetSerializable
    {
        //
        //  Variables
        //

        private NetPeer peer;

        //
        //  Construction & Config
        //

        public void Setup(NetPeer peer)
        {
            // NOTE: We use a method for construction here instead of an actual constructor because
            // of inheritence restrictions & generics restrictions.
            this.peer = peer;
        }


        //
        //  Public Methods
        //

        public NetPeer GetPeer() => this.peer;

        // If peer has been assigned, return it's ID. Otherwise, return -1.
        public int GetID() => peer != null ? this.peer.Id : -1;

        //
        //  Abstract Methods
        //

        // Apply the contents of a player update to this player.
        public abstract void ApplyPlayerUpdate(PlayerUpdate newUpdate);

        // Should the player send out an update message right now?
        public abstract bool ShouldPlayerUpdate();

        // Internally marks that we have committed to sending out an update message, and we should
        // send the result of this call.
        public abstract PlayerUpdate CommitToPlayerUpdate();

        // Creates a player update from the current player state.        
        public abstract PlayerUpdate CreatePlayerUpdate();
    }
}