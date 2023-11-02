using System;
using AppKit;
using Foundation;
using System.Diagnostics;

namespace FTAnalyzer.ViewControllers
{
    public partial class TreeTopsViewControllerController : NSViewController
    {
        // Called when created from unmanaged code
        public TreeTopsViewControllerController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
        
        partial void ShowTreetopsClicked(NSObject sender)
        {
            Debug.WriteLine("Button Clicked");
        }
    }
}
