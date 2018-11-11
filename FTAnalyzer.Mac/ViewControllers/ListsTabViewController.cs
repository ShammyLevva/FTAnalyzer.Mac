using System;
using Foundation;
using AppKit;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.Mac
{
    public partial class ListsTabViewController : NSTabViewController
    {
        AppDelegate App => (AppDelegate)NSApplication.SharedApplication.Delegate;

        public ListsTabViewController(IntPtr handle) : base(handle)
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
                case "MainListsController":
                    MainLists(item.Label);
                    break;
                case "ErrorsFixesTabController":
                    ErrorsFixes(item.Label);
                    break;
                case "LocationsTabController":
                    Locations(item.Label);
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
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.DataErrorsTabEvent);
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

        void Locations(string label)
        {
            switch (label)
            {
                case "Countries":
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.CountriesTabEvent);
                    break;
                case "Regions":
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.RegionsTabEvent);
                    break;
                case "SubRegions":
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.SubRegionsTabEvent);
                    break;
                case "Addresses":
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.AddressesTabEvent);
                    break;
                case "Places":
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.PlacesTabEvent);
                    break;
            }
        }    
    }
}