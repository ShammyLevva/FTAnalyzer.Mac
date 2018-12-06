﻿using System;
using Foundation;
using AppKit;
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
            switch (item.Label)
            {
                case "Gedcom Stats":
                    App.CurrentViewController = ChildViewControllers[0];
                    break;
                case "Main Lists":
                    App.Document.LoadMainLists(ProgressController);
                    App.CurrentViewController = ChildViewControllers[1]?.ChildViewControllers[0];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.MainListsEvent);
                    break;
                case "Errors/Fixes":
                    App.Document.LoadErrorsAndFixes(ProgressController);
                    App.CurrentViewController = ChildViewControllers[2]?.ChildViewControllers[0];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.ErrorsFixesEvent);
                    break;
                case "Locations":
                    App.Document.LoadLocations(ProgressController);
                    App.CurrentViewController = ChildViewControllers[3]?.ChildViewControllers[0];
                    Analytics.TrackAction(Analytics.MainFormAction, Analytics.LocationTabViewed);
                    break;
                default:
                    App.CurrentViewController = null;
                    break;
            }
        }

        ProgressController ProgressController { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var storyboard = NSStoryboard.FromName("Main", null);
            ProgressController = storyboard.InstantiateControllerWithIdentifier("ProgressDisplay") as ProgressController;
            ProgressController.Presentor = this;
        }
    }
}