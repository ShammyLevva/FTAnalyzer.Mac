using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace FTAnalyzer.Mac
{
    [Register ("FTAnalyzerTabViewController")]
    public class FTAnalyzerTabViewController : NSTabViewController
    {
        #region Constructors

        // Called when created from unmanaged code
        public FTAnalyzerTabViewController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public FTAnalyzerTabViewController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public FTAnalyzerTabViewController() : base("FTAnalyzerTabViewController", NSBundle.MainBundle)
        {
            Initialize();
        }

        protected FTAnalyzerTabViewController(NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        public FTAnalyzerTabViewController(string nibNameOrNull, NSBundle nibBundleOrNull) : base(nibNameOrNull, nibBundleOrNull)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        [Export("tabView:didSelectTabViewItem:")]
        public override void DidSelect(NSTabView tabView, NSTabViewItem item)
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}
