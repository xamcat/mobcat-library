﻿using Microsoft.MobCAT.Abstractions;

namespace Microsoft.MobCAT.Forms.Services
{
    public class PreferencesService : IPreferencesService
    {
        public bool GetBool(string key, bool defaultValue)
            => Xamarin.Essentials.Preferences.Get(key, defaultValue);

        public double GetDouble(string key, double defaultValue)
            => Xamarin.Essentials.Preferences.Get(key, defaultValue);

        public float GetFloat(string key, float defaultValue)
            => Xamarin.Essentials.Preferences.Get(key, defaultValue);

        public int GetInt(string key, int defaultValue)
            => Xamarin.Essentials.Preferences.Get(key, defaultValue);

        public string GetString(string key, string defaultValue)
            => Xamarin.Essentials.Preferences.Get(key, defaultValue);

        public void SetBool(string key, bool value)
            => Xamarin.Essentials.Preferences.Set(key, value);

        public void SetDouble(string key, double value)
            => Xamarin.Essentials.Preferences.Set(key, value);

        public void SetFloat(string key, float value)
            => Xamarin.Essentials.Preferences.Set(key, value);

        public void SetInt(string key, int value)
            => Xamarin.Essentials.Preferences.Set(key, value);

        public void SetString(string key, string value)
            => Xamarin.Essentials.Preferences.Set(key, value);
    }
}