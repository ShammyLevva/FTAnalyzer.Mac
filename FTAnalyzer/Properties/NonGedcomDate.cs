namespace FTAnalyzer.Properties {

    class NonGedcomDate
    {
        public static NonGedcomDate Default { get; } = (NonGedcomDate)SettingsBase.Load(new NonGedcomDate());

        [DefaultSettingValue("False")]
        public bool UseNonGedcomDates { get; set; }

        [DefaultSettingValue("dd/MM/yyyy")]
        public string DateFormat { get; set; }

        [DefaultSettingValue("^\\d{1,2}\\/\\d{1,2}\\/\\d{4}$")]
        public string Regex { get; set; }

        [DefaultSettingValue("/")]
        public string Separator { get; set; }

        [DefaultSettingValue("1")]
        public int FormatSelected { get; set; }

        [DefaultSettingValue("")]
        public string Setting { get; set; }
    }
}
