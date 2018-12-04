using System;

namespace FTAnalyzer.Properties {
    
    public class GeneralSettings
    {
        public static GeneralSettings Default { get; } = (GeneralSettings)SettingsBase.Load(new GeneralSettings());

        [DefaultSettingValue("True")]
        public bool UseBaptismDates { get; set; }

        [DefaultSettingValue("")]
        public string SavePath { get; set; }

        [DefaultSettingValue("False")]
        public bool AllowEmptyLocations { get; set; }

        [DefaultSettingValue("True")]
        public bool UseResidenceAsCensus { get; set; }

        [DefaultSettingValue("True")]
        public bool TolerateInaccurateCensusDate { get; set; }

        [DefaultSettingValue("True")]
        public bool ReloadRequired { get; set; }

        [DefaultSettingValue("False")]
        public bool ReportOptions { get; set; }

        [DefaultSettingValue("16")]
        public int MinParentalAge { get; set; }

        [DefaultSettingValue("False")]
        public bool OnlyCensusParents { get; set; }

        [DefaultSettingValue("True")]
        public bool UseBurialDates { get; set; }

        [DefaultSettingValue("True")]
        public bool MultipleFactForms { get; set; }

        [DefaultSettingValue("False")]
        public bool UseCompactCensusRef { get; set; }

        [DefaultSettingValue("False")]
        public bool ShowAliasInName { get; set; }

        [DefaultSettingValue("True")]
        public bool HidePeopleWithMissingTag { get; set; }

        [DefaultSettingValue("False")]
        public bool ReverseLocations { get; set; }

        [DefaultSettingValue("True")]
        public bool AutoCreateCensusFacts { get; set; }

        [DefaultSettingValue("True")]
        public bool ShowWorldEvents { get; set; }

        [DefaultSettingValue("True")]
        public bool AddCreatedLocations{ get; set; }

        [DefaultSettingValue("False")]
        public bool IgnoreFactTypeWarnings { get; set; }

        [DefaultSettingValue("True")]
        public bool TreatFemaleSurnamesAsUnknown { get; set; }

        [DefaultSettingValue("False")]
        public bool ShowMultiAncestors { get; set; }

        [DefaultSettingValue("False")]
        public bool SkipCensusReferences { get; set; }

        [DefaultSettingValue("True")]
        public bool HideIgnoredDuplicates { get; set; }
    }
}
