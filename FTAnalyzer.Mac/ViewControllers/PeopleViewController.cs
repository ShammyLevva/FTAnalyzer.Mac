using Foundation;
using FTAnalyzer.Mac.ViewControllers;
using AppKit;
using FTAnalyzer.Utilities;
using System;

namespace FTAnalyzer.Mac
{
    public partial class PeopleViewController : NSSplitViewController
    {
        BindingListViewController<IDisplayIndividual> IndividualsViewController { get; }
        BindingListViewController<IDisplayFamily> FamiliesViewController { get; }
        AppDelegate App => (AppDelegate)NSApplication.SharedApplication.Delegate;

        public PeopleViewController(IntPtr handle) : base(handle) 
        {
            IndividualsViewController = new BindingListViewController<IDisplayIndividual>(string.Empty, string.Empty);
            FamiliesViewController = new BindingListViewController<IDisplayFamily>(string.Empty, string.Empty);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            foreach(NSSplitViewItem item in SplitViewItems)
            {
                RemoveSplitViewItem(item); // we need to remove any old items to re-add the updated controllers
            }
            IndividualView.ViewController = IndividualsViewController;
            FamilyView.ViewController = FamiliesViewController;
            InsertSplitViewItem(IndividualView,0);
            InsertSplitViewItem(FamilyView, 1);
            IndividualsViewController.IndividualFactRowClicked += IndividualsFactRowClicked;
            FamiliesViewController.FamilyFactRowClicked += FamiliesFactRowClicked;
        }

        public void LoadIndividuals(SortableBindingList<IDisplayIndividual> list)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => LoadIndividuals(list));
                return;
            }
            IndividualView.Collapsed = false;
            IndividualsViewController.RefreshDocumentView(list);
         }

        public void LoadFamilies(SortableBindingList<IDisplayFamily> list)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => LoadFamilies(list));
                return;
            }
            FamilyView.Collapsed = false;
            FamiliesViewController.RefreshDocumentView(list);
        }

        public void HideIndividuals()
        {
            IndividualView.Collapsed = true;
        }
      
        public void HideFamilies()
        {
            FamilyView.Collapsed = true;
        }

        public void SortIndividuals()
        {
            NSSortDescriptor[] descriptors = 
            {
                new NSSortDescriptor("Surname", true),
                new NSSortDescriptor("Forename", true)
            };
            IndividualsViewController.Sort(descriptors);
        }

        public void SortFamilies()
        {
            NSSortDescriptor[] descriptors =
            {
                new NSSortDescriptor("FamilyID", true)
            };
            FamiliesViewController.Sort(descriptors);
        }

        public bool IsIndividualViewVisible => !IndividualView.Collapsed;
        public bool IsFamilyViewVisible => !FamilyView.Collapsed;

        void IndividualsFactRowClicked(Individual individual)
        {
            InvokeOnMainThread(() =>
            {
                App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {individual.IndividualID}: {individual.Name}", individual));
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsIndividualsEvent);
            });
        }

        void FamiliesFactRowClicked(Family family)
        {
            InvokeOnMainThread(() =>
            {
                App.ShowFacts(new FactsViewController<IDisplayFact>($"Facts Report for {family.FamilyRef}", family));
                Analytics.TrackAction(Analytics.FactsFormAction, Analytics.FactsFamiliesEvent);
            });
        }

    }
}
