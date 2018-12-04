namespace FTAnalyzer.Properties
{ 
    public class Settings : SettingsBase 
    {
        public static Settings Default { get; } = (Settings)Load(new Settings());

        [DefaultSettingValue("00000000-0000-0000-0000-000000000000")]
        public string GUID { get; set; }
    }
}
