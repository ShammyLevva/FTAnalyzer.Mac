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
        }

        public void RefreshIndividuals(SortableBindingList<IDisplayIndividual> list)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => RefreshIndividuals(list));
                return;
            }
            IndividualView.Collapsed = false;
            IndividualsViewController.RefreshDocumentView(list);
         }

        public void RefreshFamilies(SortableBindingList<IDisplayFamily> list)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => RefreshFamilies(list));
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
    }
}
