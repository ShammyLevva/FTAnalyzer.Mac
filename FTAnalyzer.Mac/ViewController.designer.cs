// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace FTAnalyzer.Mac
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSProgressIndicator FamiliesProgress { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator IndividualsProgress { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator RelationshipsProgress { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator SourcesProgress { get; set; }

		[Outlet]
		AppKit.NSTextView StatusTextView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (FamiliesProgress != null) {
				FamiliesProgress.Dispose ();
				FamiliesProgress = null;
			}

			if (IndividualsProgress != null) {
				IndividualsProgress.Dispose ();
				IndividualsProgress = null;
			}

			if (RelationshipsProgress != null) {
				RelationshipsProgress.Dispose ();
				RelationshipsProgress = null;
			}

			if (SourcesProgress != null) {
				SourcesProgress.Dispose ();
				SourcesProgress = null;
			}

			if (StatusTextView != null) {
				StatusTextView.Dispose ();
				StatusTextView = null;
			}
		}
	}
}
