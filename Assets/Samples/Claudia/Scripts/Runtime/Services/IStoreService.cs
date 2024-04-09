using UnityReduxMiddleware;

namespace ClaudiaApp.Services
{
    public interface IStoreService
    {
        MiddlewareStore Store { get; }
    }
}