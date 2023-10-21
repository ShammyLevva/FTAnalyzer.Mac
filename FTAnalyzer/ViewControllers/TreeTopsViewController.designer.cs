// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

using Foundation;

namespace FTAnalyzer.ViewControllers
{
	[Register ("TreeTopsViewControllerController")]
	partial class TreeTopsViewControllerController
	{
		[Outlet]
		AppKit.NSButton OnlyOneParentOutlet { get; set; }

		[Outlet]
		AppKit.NSTextField TreetopsSurnameOutlet { get; set; }

		[Action ("OnlyOneParentChecked:")]
		partial void OnlyOneParentChecked (Foundation.NSObject sender);

		[Action ("ShowTreetopsClicked:")]
		partial void ShowTreetopsClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (OnlyOneParentOutlet != null) {
				OnlyOneParentOutlet.Dispose ();
				OnlyOneParentOutlet = null;
			}

			if (TreetopsSurnameOutlet != null) {
				TreetopsSurnameOutlet.Dispose ();
				TreetopsSurnameOutlet = null;
			}
		}
	}
}
