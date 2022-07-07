using LiteNetLib.Utils;

namespace BobboNet.Networking
{
    public class NetVec3 : INetSerializable
    {
        public float X { get; set; } 
        public float Y { get; set; }
        public float Z { get; set; }

        //
        //  Constructors
        //

        public NetVec3() { }

        public NetVec3(NetVec3 otherPosition)
        {
            this.X = otherPosition.X;
            this.Y = otherPosition.Y;
            this.Z = otherPosition.Z;
        }

        //
        //  Methods
        //

        public static float DeltaMagnitude(NetVec3 a, NetVec3 b)
        {
            // TODO - Optimize
            // Calculate the positional delta
            NetVec3 delta = new NetVec3
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
                Z = a.Z - b.Z
            };

            return (delta.X * delta.X) + (delta.Y * delta.Y) + (delta.Z * delta.Z);
        }

        public float SqrMagnitude()
        {
            return (X * X) + (Y * Y) + (Z * Z);
        }

        //
        //  Operators
        //

        public static NetVec3 operator +(NetVec3 a, NetVec3 b) => new NetVec3() {
            X = a.X + b.X,
            Y = a.Y + b.Y,
            Z = a.Z + b.Z
        };

        public static NetVec3 operator *(NetVec3 a, float b) => new NetVec3() {
            X = a.X * b,
            Y = a.Y * b,
            Z = a.Z * b
        };

        //
        //  Serialization
        //

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
            writer.Put(Z);
        }

        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetFloat();
            Y = reader.GetFloat();
            Z = reader.GetFloat();
        }
    }
}