using System;
using Foundation;

namespace FTAnalyzer.Utilities
{
    public static partial class ExtensionMethods
    {

        [System.Runtime.InteropServices.DllImport("/System/Library/Frameworks/Foundation.framework/Foundation")]
        public static extern IntPtr NSHomeDirectory();

        public static string ContainerDirectory
        {
            get
            {
                return ((NSString)ObjCRuntime.Runtime.GetNSObject(NSHomeDirectory())).ToString();
            }
        }
    }
}
