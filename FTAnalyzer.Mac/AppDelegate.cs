using System;
using System.Net;
using System.Web;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using FTAnalyzer.Utilities;
using HtmlAgilityPack;

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
            window.Title = $"Family Tree Analyzer v{Version}";
            CheckWebVersion();
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

        public string Version
        {
            get
            {
                var version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();
                var build = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleVersion").ToString();
                return version + "." + build;
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
                string webVersion = versionNode.InnerText;
                if (webVersion != Version)
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

    }
}
