using System;
using AppKit;
using Foundation;

namespace FTAnalyzer.Properties
{
    [Register ("FileHandling")]
    public class FileHandling : NSViewController
    {
        public FileHandling(IntPtr handle) : base(handle) { }

        public static FileHandling Default { get; } = (FileHandling)SettingsBase.Load(new FileHandling(new IntPtr()), typeof(FileHandling));

        [DefaultSettingValue("False")]
        public bool LoadWithFilters { get; set; }

        [DefaultSettingValue("True")]
        public bool RetryFailedLines { get; set; }

        public void Save() => SettingsBase.Save(Default, typeof(FileHandling));
    }
}
