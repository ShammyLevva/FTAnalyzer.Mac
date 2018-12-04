using System;
using AppKit;
using Foundation;

namespace FTAnalyzer.Properties 
{
    [Register ("NonGedcomDate")]
    public class NonGedcomDate : NSViewController
    {
        public NonGedcomDate(IntPtr handle) : base(handle) { }

        public static NonGedcomDate Default { get; } = (NonGedcomDate)SettingsBase.Load(new NonGedcomDate(new IntPtr()), typeof(NonGedcomDate));

        [DefaultSettingValue("False")]
        public bool UseNonGedcomDates { get; set; }

        [DefaultSettingValue(typeof(string), "dd/MM/yyyy")]
        public string DateFormat { get; set; }

        [DefaultSettingValue(typeof(string), "^\\d{1,2}\\/\\d{1,2}\\/\\d{4}$")]
        public string Regex { get; set; }

        [DefaultSettingValue(typeof(string), "/")]
        public string Separator { get; set; }

        [DefaultSettingValue(typeof(int), "1")]
        public int FormatSelected { get; set; }

        [DefaultSettingValue(typeof(string), "")]
        public string Setting { get; set; }

        public void Save() => SettingsBase.Save(Default, typeof(NonGedcomDate));
    }
}
