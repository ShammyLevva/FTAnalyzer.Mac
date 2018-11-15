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
            IndividualView.ViewController = IndividualsViewController;
            FamilyView.ViewController = FamiliesViewController;
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
    }
}
