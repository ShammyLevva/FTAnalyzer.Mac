using System;
using AppKit;
using Foundation;

namespace FTAnalyzer.Properties
{
    [Register ("GeneralSettings")]
    public class GeneralSettings : NSViewController
    {
        public GeneralSettings(IntPtr handle) : base(handle) { }

        public static GeneralSettings Default { get; } = (GeneralSettings)SettingsBase.Load(new GeneralSettings(new IntPtr()), typeof(GeneralSettings));

        [DefaultSettingValue("True")]
        public bool UseBaptismDates { get; set; }

        [DefaultSettingValue(typeof(string), "")]
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

        [DefaultSettingValue(typeof(int), "16")]
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

        public void Save() => SettingsBase.Save(Default, typeof(GeneralSettings));
    }
}
