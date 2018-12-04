using System;
using System.Reflection;
using Foundation;

namespace FTAnalyzer.Properties
{
    public static class SettingsBase
    {
        public static object Load(object settings, Type type)
        {
            var output = Convert.ChangeType(settings, type);
            PropertyInfo[] properties = type.GetProperties();
            var userDefaults = new NSUserDefaults();
            foreach (var property in properties)
            {
                var defaultValue = property.GetCustomAttributes(typeof(DefaultSettingValueAttribute), false) as DefaultSettingValueAttribute[];
                if (defaultValue.Length > 0) // only process fields with a custom attribute
                {
                    Type fieldtype = defaultValue[0].DefaultType;
                    string param = userDefaults.StringForKey($"{type.ToString()}.{property.Name}");
                    string value = param ?? defaultValue[0].DefaultValue;
                    PropertyInfo outputField = output.GetType().GetProperty(property.Name);
                    outputField.SetValue(output, Convert.ChangeType(value, fieldtype));
                }
            }
            Save(settings, type); // make sure defaults are saved if not already set.
            return output;
        }

        public static void Save(object settings, Type type)
        {
            var output = Convert.ChangeType(settings, type);
            PropertyInfo[] properties = type.GetProperties();
            var userDefaults = new NSUserDefaults();
            foreach (var property in properties)
            {
                var defaultValue = property.GetCustomAttributes(typeof(DefaultSettingValueAttribute), false) as DefaultSettingValueAttribute[];
                if (defaultValue.Length > 0) // only process fields with a custom attribute
                {
                    string value = property.GetValue(settings).ToString();
                    userDefaults.SetString(value, $"{type.ToString()}.{property.Name}");
                }
            }
            userDefaults.Synchronize();
        }
    }
}
