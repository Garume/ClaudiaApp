using Unity.AppUI.Redux;

namespace ClaudiaApp.Services
{
    public class StoreService : IStoreService
    {
        public StoreService()
        {
            Store = new Store();
        }

        public Store Store { get; }
    }
}