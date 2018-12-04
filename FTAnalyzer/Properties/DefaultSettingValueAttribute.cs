using System;
namespace FTAnalyzer.Properties
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class DefaultSettingValueAttribute : Attribute
    {
        public string DefaultValue { get; }
        public Type DefaultType { get; }

        public DefaultSettingValueAttribute(string defaultValue) : this(typeof(bool), defaultValue) { }
        public DefaultSettingValueAttribute(Type type, string defaultValue)
        {
            DefaultType = type;
            DefaultValue = defaultValue;
        }
    }
}
