// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace FTAnalyzer.ViewControllers
{
	[Register ("ListsTabViewController")]
	partial class ListsTabViewController
	{
		[Outlet]
		AppKit.NSMenu SetRootPersonMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem SetRootPersonMenuItem { get; set; }

		[Action ("SetRootPersonClicked:")]
		partial void SetRootPersonClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (SetRootPersonMenuItem != null) {
				SetRootPersonMenuItem.Dispose ();
				SetRootPersonMenuItem = null;
			}

			if (SetRootPersonMenu != null) {
				SetRootPersonMenu.Dispose ();
				SetRootPersonMenu = null;
			}
		}
	}
}
