using System;
using AppKit;
using Foundation;

namespace FTAnalyzer.Utilities
{
    public class PrintDelegate : NSObject
    {
        public bool Success { get; set; }

        [Export("printOperationDidRun:success:contextInfo:")]
        public void PrintOperationDidRun(IntPtr printOperation, bool success, IntPtr contextInfo)
        {
            Success = success;
            NSApplication.SharedApplication.StopModalWithCode(success ? 1 : 0);
        }
    }
}
