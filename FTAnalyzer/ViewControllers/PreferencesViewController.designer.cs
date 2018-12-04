// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace FTAnalyzer.ViewControllers
{
	[Register ("PreferencesViewController")]
	partial class PreferencesViewController
	{
		[Outlet]
		AppKit.NSButton LoadWithFiltersOutlet { get; set; }

		[Outlet]
		AppKit.NSButton RetryFailedLinesOutlet { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LoadWithFiltersOutlet != null) {
				LoadWithFiltersOutlet.Dispose ();
				LoadWithFiltersOutlet = null;
			}

			if (RetryFailedLinesOutlet != null) {
				RetryFailedLinesOutlet.Dispose ();
				RetryFailedLinesOutlet = null;
			}
		}
	}
}
