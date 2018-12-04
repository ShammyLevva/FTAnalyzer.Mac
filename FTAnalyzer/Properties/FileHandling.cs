namespace FTAnalyzer.Properties
{

    public class FileHandling
    {
        public static FileHandling Default { get; } = (FileHandling)SettingsBase.Load(new FileHandling(), typeof(FileHandling));

        [DefaultSettingValue("False")]
        public bool LoadWithFilters { get; set; }

        [DefaultSettingValue("False")]
        public bool RetryFailedLines { get; set; }
    }
}
