// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace FTAnalyzer.ViewControllers
{
	[Register ("PeopleViewController")]
	partial class PeopleViewController
	{
		[Outlet]
		AppKit.NSSplitViewItem FamilyView { get; set; }

		[Outlet]
		AppKit.NSSplitViewItem IndividualView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (FamilyView != null) {
				FamilyView.Dispose ();
				FamilyView = null;
			}

			if (IndividualView != null) {
				IndividualView.Dispose ();
				IndividualView = null;
			}
		}
	}
}
