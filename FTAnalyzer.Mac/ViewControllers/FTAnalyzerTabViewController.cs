using System;
using Foundation;
using AppKit;
using FTAnalyzer.Utilities;
using CoreGraphics;

namespace FTAnalyzer.Mac
{
    public partial class FTAnalyzerTabViewController : NSTabViewController
    {
        AppDelegate App => (AppDelegate)NSApplication.SharedApplication.Delegate;

        public FTAnalyzerTabViewController(IntPtr handle) : base(handle)
        {
        }

        [Export("tabView:didSelectTabViewItem:")]
        public override void DidSelect(NSTabView tabView, NSTabViewItem item)
        {
            if (App.Document == null)
                return; // don't bother if we've not loaded a document yet
            Console.WriteLine($"TabView: {Title}");
            Console.WriteLine($"Item: {item.Label}");
            switch (Title)
            {
                case "MainTabController":
                    MainForm(item.Label);
                    break;
                case "MainListsController":
                    MainLists(item.Label);
                    break;
                case "ErrorsFixesTabController":
                    ErrorsFixes(item.Label);
                    break;
                case "LocationsTabController":
                    //Locations(item.Label);
                    break;
            }
        }

        NSProgressIndicator ProgressBar { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (Title == "MainTabController")
            {
                ProgressBar = new NSProgressIndicator(new CGRect(390, 0, 490, 20))
                {
                    MinValue = 0,
                    MaxValue = 100,
                    DoubleValue = 0,
                    Hidden = false,
                    Indeterminate = true
                };
                TabView.AddSubview(ProgressBar);
            }
        }

        void MainForm(string label)
        {
            
            switch (label)
            {
                case "Gedcom Stats":
                    break;
                case "Main Lists":
                    App.Document.LoadMainLists(ProgressBar);
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.MainListsEvent);
                    break;
                case "Errors/Fixes":
                    App.Document.LoadErrorsAndFixes(ProgressBar);
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.ErrorsFixesEvent);
                    break;
                case "Locations":
                    App.Document.LoadLocations(ProgressBar);
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LocationTabViewed);
                    break;
            }
        }

        void MainLists(string label)
        {
            switch (label)
            {
                case "Individuals":
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.IndividualsTabEvent);
                    break;
                case "Families":
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.FamilyTabEvent);
                    break;
                case "Sources":
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.SourcesTabEvent);
                    break;
                case "Occupations":
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.OccupationsTabEvent);
                    break;
                case "Facts":
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.FactsTabEvent);
                    break;
            }
        }

        void ErrorsFixes(string label)
        {
            switch (label)
            {
                case "Data Errors":
                    break;
                case "Duplicates":
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.DuplicatesTabEvent);
                    break;
                case "Loose Births":
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LooseBirthsEvent);
                    break;
                case "Loose Deaths":
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LooseDeathsEvent);
                    break;
            }
        }

        //void Locations(string label)
        //{
        //    switch (label)
        //    {
        //        case "Countries":
        //            break;
        //        case "Regions":
        //            //Analytics.TrackAction(Analytics.MainFormAction, Analytics.);
        //            break;
        //        case "SubRegions":
        //            Analytics.TrackAction(Analytics.MainFormAction, Analytics.ErrorsFixesEvent);
        //            break;
        //        case "Addresses":
        //            Analytics.TrackAction(Analytics.MainFormAction, Analytics.LocationTabViewed);
        //            break;
        //        case "Places":
        //            Analytics.TrackAction(Analytics.MainFormAction, Analytics.LocationTabViewed);
        //            break;
        //    }
        //}    
    }
}