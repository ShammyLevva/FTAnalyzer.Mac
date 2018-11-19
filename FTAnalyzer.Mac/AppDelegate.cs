using System;
using System.Net;
using System.Web;
using AppKit;
using Foundation;
using FTAnalyzer.Mac.ViewControllers;
using FTAnalyzer.Utilities;
using HtmlAgilityPack;

namespace FTAnalyzer.Mac
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        bool _documentOpening;
        public GedcomDocument Document { get; set; }

        //static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
            var window = NSApplication.SharedApplication.Windows[0];
            window.MakeKeyAndOrderFront(Self);
            window.Title = $"Family Tree Analyzer {Version}";
            window.Delegate = new MainWindowDelegate();
            //var tabs = window.ContentViewController as NSTabViewController;
            //tabs.SelectedTabViewItemIndex = 0; //set tab to GEDCOM stats
            CheckWebVersion();
        }

        class MainWindowDelegate : NSWindowDelegate
        {
            public override void WillClose(NSNotification notification)
            {
                foreach (NSWindow window in NSApplication.SharedApplication.Windows)
                {
                    if (window != NSApplication.SharedApplication.Windows[0])
                    {
                        window.Close();
                        window.Dispose();
                    }
                }
            }
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
                CloseAllFactsWindows();
                _documentOpening = false;
            }
            return false;
        }

        public void ShowFacts(NSViewController factsViewController)
        {
            var storyboard = NSStoryboard.FromName("Facts", null);
            var factsWindow = storyboard.InstantiateControllerWithIdentifier("FactsWindow") as NSWindowController;
            factsWindow.ContentViewController.AddChildViewController(factsViewController);
            factsWindow.Window.Title = factsViewController.Title;
            factsWindow.Window.SetFrame(new CoreGraphics.CGRect(350, 350, 800, 500), true);
            factsWindow.ShowWindow(this);
        }

        public void CloseAllFactsWindows()
        {
            var storyboard = NSStoryboard.FromName("Facts", null);
            foreach(NSWindow window in NSApplication.SharedApplication.Windows)
            {
                if (window.Title.Length >= 5 && window.Title.Substring(0,5) == "Facts")
                    window.Close();
            }
        }

        public string Version
        {
            get
            {
                var version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
                var build = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
                return $"v{version}.{build}";
            }
        }

        async void CheckWebVersion()
        {
            try
            {
                WebClient wc = new WebClient();
                HtmlDocument doc = new HtmlDocument();
                string webData = wc.DownloadString("https://github.com/ShammyLevva/FTAnalyzer.Mac/releases");
                doc.LoadHtml(webData);
                HtmlNode versionNode = doc.DocumentNode.SelectSingleNode("//span[@class='css-truncate-target']/text()");
                Version webVersion = new Version(versionNode.InnerText.Substring(1));
                Version programVersion = new Version(Version.Substring(1));
                if (webVersion > programVersion)
                {
                    string text = $"Version installed: {Version}, Web version available: {webVersion}\nDo you want to go to website to download the latest version?";
                    var download = UIHelpers.ShowYesNo(text, "FTAnalyzer");
                    if (download == UIHelpers.Yes)
                        HttpUtility.VisitWebsite("https://github.com/ShammyLevva/FTAnalyzer.Mac/releases");
                }
                await Analytics.CheckProgramUsageAsync();
            }
            catch (Exception e) 
                { Console.WriteLine(e.Message); }
        }

        partial void ViewOnlineManual(NSObject sender)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.OnlineManualEvent);
            HttpUtility.VisitWebsite("http://www.ftanalyzer.com");
        }

        partial void ViewOnlineGUides(NSObject sender)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.OnlineGuideEvent);
            HttpUtility.VisitWebsite("http://www.ftanalyzer.com/guides");
        }

        partial void ReportIssue(NSObject sender)
        {
            UIHelpers.ShowMessage("Please note this is a early development version if you find a crashing bug please report it.\nOtherwise assume I'll get round to fixing things later.\nYou may find it more useful to raise issue at the Facebook User Group.");
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.ReportIssueEvent);
            HttpUtility.VisitWebsite("https://github.com/ShammyLevva/FTAnalyzer.Mac/issues");
        }

        partial void VisitFacebookSupport(NSObject sender)
        {
            HttpUtility.VisitWebsite("https://www.facebook.com/ftanalyzer");
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.FacebookSupportEvent);
        }

        partial void VisitFacebookUserGroup(NSObject sender)
        {
            HttpUtility.VisitWebsite("https://www.facebook.com/groups/ftanalyzer");
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.FacebookUsersEvent);
        }

        partial void VisitPrivacyPolicy(NSObject sender)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.PrivacyEvent);
            HttpUtility.VisitWebsite("http://www.ftanalyzer.com/privacy");
        }

        partial void VisitWhatsNew(NSObject sender)
        {
            Analytics.TrackAction(Analytics.MainFormAction, Analytics.WhatsNewEvent);
            HttpUtility.VisitWebsite("http://mac.ftanalyzer.com/Whats%20New%20in%20this%20Release");
        }
    }
}
