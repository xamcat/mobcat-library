using Microsoft.MobCAT.Abstractions;
using Microsoft.Maui.Storage;

namespace Microsoft.MobCAT.Forms.Services
{
    public class PreferencesService : IPreferencesService
    {
        public bool GetBool(string key, bool defaultValue)
            => Preferences.Get(key, defaultValue);

        public double GetDouble(string key, double defaultValue)
            => Preferences.Get(key, defaultValue);

        public float GetFloat(string key, float defaultValue)
            => Preferences.Get(key, defaultValue);

        public int GetInt(string key, int defaultValue)
            => Preferences.Get(key, defaultValue);

        public string GetString(string key, string defaultValue)
            => Preferences.Get(key, defaultValue);

        public void SetBool(string key, bool value)
            => Preferences.Set(key, value);

        public void SetDouble(string key, double value)
            => Preferences.Set(key, value);

        public void SetFloat(string key, float value)
            => Preferences.Set(key, value);

        public void SetInt(string key, int value)
            => Preferences.Set(key, value);

        public void SetString(string key, string value)
            => Preferences.Set(key, value);
    }
}