using System;
using System.Globalization;
using System.Threading.Tasks;
using AppKit;
using GoogleAnalyticsTracker.Core;
using GoogleAnalyticsTracker.Core.TrackerParameters;
using GoogleAnalyticsTracker.Simple;

namespace FTAnalyzer.Utilities
{
    public static class SpecialMethodsOld
    {
        public static async Task<TrackingResult> TrackEventAsync(this SimpleTracker tracker, string category, string action, string label, long value = 1)
        {
            string resolution = NSScreen.MainScreen.Frame.ToString();
            var eventTrackingParameters = new EventTracking
            {
                ClientId = Analytics.GUID,
                UserId = Analytics.GUID,
                ApplicationName = "FTAnalyzer",
                ApplicationVersion = Analytics.AppVersion,
                Category = category,
                Action = action,
                Label = label,
                Value = value,
                ScreenName = category,
                ScreenResolution = resolution.Length > 11 ? resolution.Substring(9, resolution.Length - 10) : resolution,
                CacheBuster = tracker.AnalyticsSession.GenerateCacheBuster(),
                CustomDimension1 = Analytics.DeploymentType,
                CustomDimension2 = Analytics.OSVersion,
                GoogleAdWordsId = "201-455-7333",
                UserLanguage = CultureInfo.CurrentUICulture.EnglishName
            };
            return await tracker.TrackAsync(eventTrackingParameters);
        }

        public static async Task<TrackingResult> TrackScreenviewAsync(this SimpleTracker tracker, string screen)
        {
            string resolution = NSScreen.MainScreen.Frame.ToString();
            var screenViewTrackingParameters = new ScreenviewTracking
            {
                ClientId = Analytics.GUID,
                UserId = Analytics.GUID,

                ApplicationName = "FTAnalyzer",
                ApplicationVersion = Analytics.AppVersion,
                ScreenName = screen,
                ScreenResolution = resolution.Length > 11 ? resolution.Substring(9, resolution.Length - 10) : resolution,
                CacheBuster = tracker.AnalyticsSession.GenerateCacheBuster(),
                CustomDimension1 = Analytics.DeploymentType,
                CustomDimension2 = Analytics.OSVersion,
                GoogleAdWordsId = "201-455-7333",
                UserLanguage = CultureInfo.CurrentUICulture.EnglishName
            };
            return await tracker.TrackAsync(screenViewTrackingParameters);
        }
    }
}