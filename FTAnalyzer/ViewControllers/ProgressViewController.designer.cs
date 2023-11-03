// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
namespace FTAnalyzer.ViewControllers
{
	[Register ("ProgressViewController")]
	partial class ProgressViewController
	{
		[Outlet]
        NSProgressIndicator ProgressBar { get; set; }

		[Outlet]
		NSTextField ProgressText { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ProgressText != null) {
				ProgressText.Dispose ();
				ProgressText = null;
			}

			if (ProgressBar != null) {
				ProgressBar.Dispose ();
				ProgressBar = null;
			}
		}
	}
}
