using System;
using AppKit;
using Foundation;
using FTAnalyzer.ViewControllers;
using FTAnalyzer.Utilities;

namespace FTAnalyzer
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

        public void Print(NSObject sender)
        {
            try
            {
                var printInfo = new NSPrintInfo
                {
                    Orientation = NSPrintingOrientation.Landscape,
                    LeftMargin = 45,
                    RightMargin = 30,
                    TopMargin = 30,
                    BottomMargin = 30,
                    HorizontalPagination = NSPrintingPaginationMode.Auto,
                    VerticallyCentered = false,
                    HorizontallyCentered = false
                };
                var printView = new NSView
                {
                    AutoresizesSubviews = true
                };
                var indPrintVC = new TablePrintingViewController(_individualsViewController);
                var famPrintVC = new TablePrintingViewController(_familiesViewController);
                indPrintVC.View.SetFrameOrigin(new CoreGraphics.CGPoint(0, famPrintVC.TotalHeight));
                printView.AddSubview(indPrintVC.View);
                printView.AddSubview(famPrintVC.View);

                 var width = Math.Max(indPrintVC.TotalWidth, famPrintVC.TotalWidth);
                var height = indPrintVC.TotalHeight + famPrintVC.TotalHeight;
                printView.SetFrameSize(new CoreGraphics.CGSize(width, height));

                var printOperation = NSPrintOperation.FromView(printView, printInfo);
                printOperation.ShowsPrintPanel = true;
                printOperation.ShowsProgressPanel = true;
                printOperation.CanSpawnSeparateThread = true;
                printOperation.PrintPanel.Options = NSPrintPanelOptions.ShowsCopies | NSPrintPanelOptions.ShowsPageRange | NSPrintPanelOptions.ShowsPreview |
                                                    NSPrintPanelOptions.ShowsPageSetupAccessory | NSPrintPanelOptions.ShowsScaling;
                printOperation.RunOperation();
                printOperation.CleanUpOperation();
            }
            catch (Exception e)
            {
                UIHelpers.ShowMessage($"Sorry there was a problem printing.\nError was: {e.Message}");
            }
        }
    }
}
