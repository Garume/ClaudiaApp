using UnityEngine;

namespace ClaudiaApp.Services
{
    public class LocalStorageService : ILocalStorageService
    {
        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (PlayerPrefs.HasKey(key))
            {
                var json = PlayerPrefs.GetString(key);
                return JsonUtility.FromJson<T>(json);
            }

            SetValue(key, defaultValue);
            return defaultValue;
        }

        public void SetValue<T>(string key, T value)
        {
            var json = JsonUtility.ToJson(value);
            PlayerPrefs.SetString(key, json);
        }
    }
}