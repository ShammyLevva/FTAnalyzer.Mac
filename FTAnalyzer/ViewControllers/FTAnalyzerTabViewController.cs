using System;
using AppKit;
using Foundation;
using FTAnalyzer.Utilities;

namespace FTAnalyzer
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
            NSViewController viewController;
            switch (item.Label)
            {
                case "Gedcom Stats":
                    viewController = ChildViewControllers[0];
                    break;
                case "Main Lists":
                    App.Document.LoadMainLists(ProgressController);
                    viewController = ChildViewControllers[1];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.MainListsEvent);
                    break;
                case "Errors/Fixes":
                    App.Document.LoadErrorsAndFixes(ProgressController);
                    viewController = ChildViewControllers[2];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.ErrorsFixesEvent);
                    break;
                case "Locations":
                    App.Document.LoadLocations(ProgressController);
                    viewController = ChildViewControllers[3];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LocationTabViewed);
                    break;
                default:
                    viewController = null;
                    break;
            }
            if (viewController?.ChildViewControllers.Length > 0)
                App.CurrentViewController = viewController.ChildViewControllers[0];
            else
                App.CurrentViewController = null;
        }

        ProgressController ProgressController { get; set; }
        public NSView PrintView { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var storyboard = NSStoryboard.FromName("Main", null);
            ProgressController = storyboard.InstantiateControllerWithIdentifier("ProgressDisplay") as ProgressController;
            ProgressController.Presentor = this;
        }
    }
}