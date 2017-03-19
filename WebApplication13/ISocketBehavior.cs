
namespace WebApplication13
{
    public interface ISocketBehavior
    {
        void OnBinaryMessage(byte[] message);
        void OnStringMessage(string message);
    }
}