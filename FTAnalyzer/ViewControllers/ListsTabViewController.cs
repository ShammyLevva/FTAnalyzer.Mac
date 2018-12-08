using System;
using Foundation;
using AppKit;
using FTAnalyzer.Utilities;
using FTAnalyzer.ViewControllers;

namespace FTAnalyzer
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
                    App.CurrentViewController = ChildViewControllers[0];
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.IndividualsTabEvent);
                    break;
                case "Families":
                    App.CurrentViewController = ChildViewControllers[1];
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.FamilyTabEvent);
                    break;
                case "Sources":
                    App.CurrentViewController = ChildViewControllers[2];
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.SourcesTabEvent);
                    break;
                case "Occupations":
                    App.CurrentViewController = ChildViewControllers[3];
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.OccupationsTabEvent);
                    break;
                case "Facts":
                    App.CurrentViewController = ChildViewControllers[4];
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.FactsTabEvent);
                    break;
            }
        }

        void ErrorsFixes(string label)
        {
            switch (label)
            {
                case "Data Errors":
                    App.CurrentViewController = ChildViewControllers[0];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.DataErrorsTabEvent);
                    break;
                case "Duplicates": //TODO: update child view controller values when added back
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.DuplicatesTabEvent);
                    break;
                case "Loose Births":
                    App.CurrentViewController = ChildViewControllers[1];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LooseBirthsEvent);
                    break;
                case "Loose Deaths":
                    App.CurrentViewController = ChildViewControllers[2];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LooseDeathsEvent);
                    break;
            }
        }

        void Locations(string label)
        {
            switch (label)
            {
                case "Countries":
                    App.CurrentViewController = ChildViewControllers[0];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.CountriesTabEvent);
                    break;
                case "Regions":
                    App.CurrentViewController = ChildViewControllers[1];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.RegionsTabEvent);
                    break;
                case "SubRegions":
                    App.CurrentViewController = ChildViewControllers[2];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.SubRegionsTabEvent);
                    break;
                case "Addresses":
                    App.CurrentViewController = ChildViewControllers[3];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.AddressesTabEvent);
                    break;
                case "Places":
                    App.CurrentViewController = ChildViewControllers[4];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.PlacesTabEvent);
                    break;
            }
        }    
    }
}