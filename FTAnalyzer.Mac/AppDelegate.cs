using System;
using AppKit;
using Foundation;

namespace FTAnalyzer.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        bool _documentOpening;

        public GedcomDocument Document { get; set; }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
            var window = NSApplication.SharedApplication.Windows[0];
            window.Title = "Family Tree Analyzer v" + Version;
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return !_documentOpening;
        }

        public override bool OpenFile(NSApplication sender, string filename)
        {
            if (Document != null)
            {
                _documentOpening = true;
                Document.Close();
                _documentOpening = false;
            }

            return false;
        }

        string Version
        {
            get
            {
                var version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
                var build = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
                return version + "." + build;
            }
        }
    }
}
