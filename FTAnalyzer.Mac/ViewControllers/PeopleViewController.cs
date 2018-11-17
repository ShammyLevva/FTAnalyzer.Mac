using System;
using AppKit;
using Foundation;
using FTAnalyzer.Mac.ViewControllers;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.Mac
{
    public partial class PeopleViewController : NSSplitViewController
    {
        BindingListViewController<IDisplayIndividual> _individualsViewController;
        BindingListViewController<IDisplayFamily> _familiesViewController;
        AppDelegate App => (AppDelegate)NSApplication.SharedApplication.Delegate;

        public PeopleViewController(IntPtr handle) : base(handle) 
        {
            _individualsViewController = new BindingListViewController<IDisplayIndividual>("Individuals", "Double click to show a list of facts for the selected individual.");
            _familiesViewController = new BindingListViewController<IDisplayFamily>("Families", "Double click to show a list of facts for the selected family.");
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            foreach(NSSplitViewItem item in SplitViewItems)
                RemoveSplitViewItem(item); // we need to remove any old items to re-add the updated controllers
            IndividualView.ViewController = _individualsViewController;
            FamilyView.ViewController = _familiesViewController;
            InsertSplitViewItem(IndividualView,0);
            InsertSplitViewItem(FamilyView, 1);
            _individualsViewController.IndividualFactRowClicked += IndividualsFactRowClicked;
            _familiesViewController.FamilyFactRowClicked += FamiliesFactRowClicked;
            SplitView.AutosaveName = "SplitView";
            SplitView.ResizeSubviewsWithOldSize(new CoreGraphics.CGSize(800, 250));
        }

        public void LoadIndividuals(SortableBindingList<IDisplayIndividual> list)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => LoadIndividuals(list));
                return;
            }
            HideIndividuals(false);
            _individualsViewController.RefreshDocumentView(list);
            SortIndividuals();
         }

        public void LoadFamilies(SortableBindingList<IDisplayFamily> list)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => LoadFamilies(list));
                return;
            }
            HideFamilies(false);
            _familiesViewController.RefreshDocumentView(list);
            SortFamilies();
        }

        public void HideIndividuals(bool value) => IndividualView.Collapsed = value;

        public void HideFamilies(bool value) => FamilyView.Collapsed = value;

        public void SortIndividuals()
        {
            NSSortDescriptor[] descriptors = 
            {
                new NSSortDescriptor("Surname", true),
                new NSSortDescriptor("Forename", true)
            };
            _individualsViewController.Sort(descriptors);
        }

        public void SortFamilies()
        {
            NSSortDescriptor[] descriptors =
            {
                new NSSortDescriptor("FamilyID", true)
            };
            _familiesViewController.Sort(descriptors);
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
