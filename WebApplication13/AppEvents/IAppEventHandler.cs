
namespace WebApplication13
{
    internal interface IAppEvent
    {
        AppEventType EvenType { get; }
        void Execute_AppHandler();
    }

}