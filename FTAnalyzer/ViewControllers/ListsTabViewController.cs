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
                    App.CurrentView = ChildViewControllers[0] as IPrintView;
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.IndividualsTabEvent);
                    break;
                case "Families":
                    App.CurrentView = ChildViewControllers[1] as IPrintView;
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.FamilyTabEvent);
                    break;
                case "Sources":
                    App.CurrentView = ChildViewControllers[2] as IPrintView;
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.SourcesTabEvent);
                    break;
                case "Occupations":
                    App.CurrentView = ChildViewControllers[3] as IPrintView;
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.OccupationsTabEvent);
                    break;
                case "Facts":
                    App.CurrentView = ChildViewControllers[4] as IPrintView;
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.FactsTabEvent);
                    break;
            }
        }

        void ErrorsFixes(string label)
        {
            switch (label)
            {
                case "Data Errors":
                    App.CurrentView = ChildViewControllers[0] as IPrintView;
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.DataErrorsTabEvent);
                    break;
                case "Duplicates": //todo: update child view controller values when added back
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.DuplicatesTabEvent);
                    break;
                case "Loose Births":
                    App.CurrentView = ChildViewControllers[1] as IPrintView;
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LooseBirthsEvent);
                    break;
                case "Loose Deaths":
                    App.CurrentView = ChildViewControllers[2] as IPrintView;
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LooseDeathsEvent);
                    break;
            }
        }

        void Locations(string label)
        {
            switch (label)
            {
                case "Countries":
                    App.CurrentView = ChildViewControllers[0] as IPrintView;
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.CountriesTabEvent);
                    break;
                case "Regions":
                    App.CurrentView = ChildViewControllers[1] as IPrintView;
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.RegionsTabEvent);
                    break;
                case "SubRegions":
                    App.CurrentView = ChildViewControllers[2] as IPrintView;
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.SubRegionsTabEvent);
                    break;
                case "Addresses":
                    App.CurrentView = ChildViewControllers[3] as IPrintView;
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.AddressesTabEvent);
                    break;
                case "Places":
                    App.CurrentView = ChildViewControllers[4] as IPrintView;
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.PlacesTabEvent);
                    break;
            }
        }    
    }
}