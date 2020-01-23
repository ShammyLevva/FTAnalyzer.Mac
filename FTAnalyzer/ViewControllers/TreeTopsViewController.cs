using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

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
            Console.WriteLine("Button Clicked");
        }
    }
}
