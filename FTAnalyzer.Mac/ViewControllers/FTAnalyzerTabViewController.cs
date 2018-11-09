using System;
using Foundation;
using AppKit;
using FTAnalyzer.Utilities;

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
                case "MainTabSelector":
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

        void MainForm(string label)
        {
            switch (label)
            {
                case "Gedcom Stats":
                    break;
                case "Main Lists":
                    App.Document.LoadMainLists();
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.MainListsEvent);
                    break;
                case "Errors/Fixes":
                    App.Document.LoadErrorsAndFixes();
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.ErrorsFixesEvent);
                    break;
                case "Locations":
                    App.Document.LoadLocations();
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
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.MainListsEvent);
                    break;
                case "Loose Births":
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.ErrorsFixesEvent);
                    break;
                case "Loose Deaths":
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LocationTabViewed);
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