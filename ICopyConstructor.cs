namespace BobboNet.Networking
{
    public interface ICopyConstructor<T>
    {
        T Copy(T other);
    }
}