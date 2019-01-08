using System;
using AppKit;
using Foundation;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.ViewControllers
{
    public partial class ListsTabViewController : NSTabViewController
    {
        AppDelegate App => (AppDelegate)NSApplication.SharedApplication.Delegate;
       
        public ListsTabViewController(IntPtr handle) : base(handle)
        {
        }

        partial void SetRootPersonClicked(NSObject sender)
        {
            if (ChildViewControllers[0] is BindingListViewController<IDisplayIndividual> controller)
            {
                Individual ind = controller.GetSelectedPerson();
                if (ind == null)
                    UIHelpers.ShowMessage("You must click to select a row in the table before you can set the root person");
                else
                {
                    var outputText = new Progress<string>(App.DocumentViewController.AppendMessage);
                    FamilyTree.Instance.UpdateRootIndividual(ind.IndividualID, null, outputText);
                    if (ParentViewController is FTAnalyzerViewController FTAnalyzer)
                    {
                        App.CloseAllSubWindows();
                        FTAnalyzer.RefreshLists();
                        UIHelpers.ShowMessage($"Root Person changed to {ind.Name}");
                    }
                    else
                        UIHelpers.ShowMessage("Problem refreshing lists after setting root person");
                }
            }
        }

        [Export("tabView:didSelectTabViewItem:")]
        public override void DidSelect(NSTabView tabView, NSTabViewItem item)
        {
            if(SetRootPersonMenuItem != null)
                SetRootPersonMenuItem.Enabled = false;
            if (App.Document == null)
                return; // don't bother if we've not loaded a document yet
            switch (Title)
            {
                case "MainListsController":
                    MainLists(item.Label);
                    break;
                case "ErrorsFixesController":
                    ErrorsFixes(item.Label);
                    break;
                case "LocationsController":
                    Locations(item.Label);
                    break;
            }
        }

        void MainLists(string label)
        {
            App.CurrentViewController = null;
            switch (label)
            {
                case "Individuals":
                    if(ChildViewControllers.Length > 0)
                        App.CurrentViewController = ChildViewControllers[0];
                    if (SetRootPersonMenuItem != null)
                        SetRootPersonMenuItem.Enabled = true; // only enable on Individuals tab
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.IndividualsTabEvent);
                    break;
                case "Families":
                    if (ChildViewControllers.Length > 1)
                        App.CurrentViewController = ChildViewControllers[1];
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.FamilyTabEvent);
                    break;
                case "Sources":
                    if (ChildViewControllers.Length > 2)
                        App.CurrentViewController = ChildViewControllers[2];
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.SourcesTabEvent);
                    break;
                case "Occupations":
                    if (ChildViewControllers.Length > 3)
                        App.CurrentViewController = ChildViewControllers[3];
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.OccupationsTabEvent);
                    break;
                case "Facts":
                    if (ChildViewControllers.Length > 4)
                        App.CurrentViewController = ChildViewControllers[4];
                    Analytics.TrackAction(Analytics.MainListsAction, Analytics.FactsTabEvent);
                    break;
            }
        }

        void ErrorsFixes(string label)
        {
            App.CurrentViewController = null;
            switch (label)
            {
                case "Data Errors":
                    if (ChildViewControllers.Length > 0)
                        App.CurrentViewController = ChildViewControllers[0];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.DataErrorsTabEvent);
                    break;
                case "Duplicates": //TODO: update child view controller values when added back
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.DuplicatesTabEvent);
                    break;
                case "Loose Births":
                    if (ChildViewControllers.Length > 1)
                        App.CurrentViewController = ChildViewControllers[1];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LooseBirthsEvent);
                    break;
                case "Loose Deaths":
                    if (ChildViewControllers.Length > 2)
                        App.CurrentViewController = ChildViewControllers[2];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LooseDeathsEvent);
                    break;
            }
        }

        void Locations(string label)
        {
            App.CurrentViewController = null;
            switch (label)
            {
                case "Countries":
                    if (ChildViewControllers.Length > 0)
                        App.CurrentViewController = ChildViewControllers[0];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.CountriesTabEvent);
                    break;
                case "Regions":
                    if (ChildViewControllers.Length > 1)
                        App.CurrentViewController = ChildViewControllers[1];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.RegionsTabEvent);
                    break;
                case "Sub-Regions":
                    if (ChildViewControllers.Length > 2)
                        App.CurrentViewController = ChildViewControllers[2];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.SubRegionsTabEvent);
                    break;
                case "Addresses":
                    if (ChildViewControllers.Length > 3)
                        App.CurrentViewController = ChildViewControllers[3];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.AddressesTabEvent);
                    break;
                case "Places":
                    if (ChildViewControllers.Length > 4)
                        App.CurrentViewController = ChildViewControllers[4];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.PlacesTabEvent);
                    break;
            }
        }    
    }
}