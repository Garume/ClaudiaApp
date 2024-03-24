namespace ClaudiaApp.Services
{
    public interface ILocalStorageService
    {
        T GetValue<T>(string key, T defaultValue = default);
        void SetValue<T>(string key, T value);
    }
}