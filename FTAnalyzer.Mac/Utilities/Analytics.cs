using System;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using FTAnalyzer.Mac;
using GoogleAnalyticsTracker.Simple;

namespace FTAnalyzer.Utilities
{
    class Analytics
    {
        static readonly SimpleTrackerEnvironment trackerEnvironment;
        static readonly SimpleTracker tracker;

        public const string MainFormAction = "Main Form Action", FactsFormAction = "Facts Form Action", CensusTabAction = "Census Tab Action",
                            ReportsAction = "Reports Action", LostCousinsAction = "Lost Cousins Action", GeocodingAction = "Geocoding Action",
                            ExportAction = "Export Action", MapsAction = "Maps Action", CensusSearchAction = "Census Search Action",
                            BMDSearchAction = "BMD Search Action";

        public static string AppVersion { get; }
        public static string DeploymentType => "Mac Website";
        public static string OSVersion { get; }
        public static string GUID { get; } 
 
        static Analytics()
        {
            var userDefaults = new NSUserDefaults();
            GUID = userDefaults.StringForKey("AnalyticsKey");
            if (string.IsNullOrEmpty(GUID))
            {
                GUID = Guid.NewGuid().ToString();
                userDefaults.SetString(GUID, "AnalyticsKey");
                userDefaults.Synchronize();
            }
            NSProcessInfo info = new NSProcessInfo();
            OSVersion = $"MacOSX {info.OperatingSystemVersionString}";
            trackerEnvironment = new SimpleTrackerEnvironment("Mac OSX", info.OperatingSystemVersion.ToString(), OSVersion);
            tracker = new SimpleTracker("UA-125850339-2", trackerEnvironment);
            var app = (AppDelegate)NSApplication.SharedApplication.Delegate;
            AppVersion = app.Version;
        }

        public static async Task CheckProgramUsageAsync() // pre demise of Windows 7 add tracker to check how many machines still use old versions
        {
            try
            {
                await SpecialMethods.TrackEventAsync(tracker, "FTAnalyzer Startup", "Load Program", AppVersion).ConfigureAwait(false);
                await SpecialMethods.TrackScreenviewAsync(tracker, "FTAnalyzer Startup").ConfigureAwait(false); ;
            }
            catch (Exception e)
                { Console.WriteLine(e.Message); }
        }

        public static Task TrackAction(string category, string action) => TrackActionAsync(category, action, "default");
        public static async Task TrackActionAsync(string category, string action, string value)
        {
            try
            {
                await SpecialMethods.TrackEventAsync(tracker, category, action, value).ConfigureAwait(false);
                await SpecialMethods.TrackScreenviewAsync(tracker, category).ConfigureAwait(false); ;
            }
            catch (Exception e)
                { Console.WriteLine(e.Message); }
        }
    }
}
