namespace FTAnalyzer.Properties
{ 
    public class Settings 
    {
        public static Settings Default { get; } = (Settings)SettingsBase.Load(new Settings(), typeof(Settings));

        [DefaultSettingValue("00000000-0000-0000-0000-000000000000")]
        public string GUID { get; set; }
    }
}
