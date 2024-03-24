namespace ClaudiaApp.Services
{
    public class MockStorageService : ILocalStorageService
    {
        public T GetValue<T>(string key, T defaultValue = default)
        {
            return defaultValue;
        }

        public void SetValue<T>(string key, T value)
        {
        }
    }
}