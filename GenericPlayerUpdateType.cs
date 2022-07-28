namespace BobboNet.Networking
{
    public enum GenericPlayerUpdateType : byte
    {
        None        = 0b_0000_0000,
        Position    = 0b_0000_0001,
        Rotation    = 0b_0000_0010,
        Animation   = 0b_0000_0100,

        All         = Position | Rotation | Animation
    }
}