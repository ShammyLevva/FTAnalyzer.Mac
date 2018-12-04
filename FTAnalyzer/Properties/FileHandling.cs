using System;
using System.Reflection;

namespace FTAnalyzer.Properties {

    public class FileHandling
    {
        public static FileHandling Default { get; } = (FileHandling)SettingsBase.Load(new FileHandling(), typeof(FileHandling));

        [DefaultSettingValue("False")]
        public bool LoadWithFilters { get; set; }

        [DefaultSettingValue("False")]
        public bool RetryFailedLines { get; set; }

        public object this[string propertyName]
        {
            get
            {
                Type myType = typeof(FileHandling);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                Type myType = typeof(FileHandling);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this, value, null);
            }
        }
    }
}
