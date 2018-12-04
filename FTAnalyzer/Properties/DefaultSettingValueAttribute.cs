using System;
namespace FTAnalyzer.Properties
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class DefaultSettingValueAttribute : Attribute
    {
        public string DefaultValue { get; }

        public DefaultSettingValueAttribute(string defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}
