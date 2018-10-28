using System;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using GoogleAnalyticsTracker.Core;
using GoogleAnalyticsTracker.Core.TrackerParameters;
using GoogleAnalyticsTracker.Simple;

namespace FTAnalyzer.Utilities
{
    public static class SpecialMethods
    {
        public static async Task<TrackingResult> TrackEventAsync(this SimpleTracker tracker, string category, string action, string label, long value = 1)
        {
            var eventTrackingParameters = new EventTracking
            {
                ClientId = Properties.Settings.Default.GUID.ToString(),
                UserId = Properties.Settings.Default.GUID.ToString(),

                ApplicationName = "FTAnalyzer",
                ApplicationVersion = Analytics.AppVersion,
                Category = category,
                Action = action,
                Label = label,
                Value = value,
                ScreenName = category,
                ScreenResolution = NSScreen.MainScreen.ToString(),
                CacheBuster = tracker.AnalyticsSession.GenerateCacheBuster()
            };
            return await tracker.TrackAsync(eventTrackingParameters);
        }

        public static async Task<TrackingResult> TrackScreenviewAsync(this SimpleTracker tracker, string screen)
        {
            var screenViewTrackingParameters = new ScreenviewTracking
            {
                ClientId = Properties.Settings.Default.GUID.ToString(),
                UserId = Properties.Settings.Default.GUID.ToString(),

                ApplicationName = "FTAnalyzer",
                ApplicationVersion = Analytics.AppVersion,
                ScreenName = screen,
                ScreenResolution = NSScreen.MainScreen.ToString(),
                CacheBuster = tracker.AnalyticsSession.GenerateCacheBuster()
            };
            Console.WriteLine(Properties.Settings.Default.GUID.ToString());
            return await tracker.TrackAsync(screenViewTrackingParameters);
        }
    }
}