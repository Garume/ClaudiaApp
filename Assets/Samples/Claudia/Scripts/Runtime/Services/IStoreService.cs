using Unity.AppUI.Redux;

namespace ClaudiaApp.Services
{
    public interface IStoreService
    {
        Store Store { get; }
    }
}