// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace FTAnalyzer
{
	[Register ("RelationshipTypesView")]
	partial class RelationshipTypesView
	{
		[Outlet]
		AppKit.NSButton BloodOutlet { get; set; }

		[Outlet]
		AppKit.NSButton DescendantsOutlet { get; set; }

		[Outlet]
		AppKit.NSButton DirectOutlet { get; set; }

		[Outlet]
		AppKit.NSButton LinkedOutlet { get; set; }

		[Outlet]
		AppKit.NSButton MarriageOutlet { get; set; }

		[Outlet]
		AppKit.NSButton MarriedDBOutlet { get; set; }

		[Outlet]
		AppKit.NSButton UnknownOutlet { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DirectOutlet != null) {
				DirectOutlet.Dispose ();
				DirectOutlet = null;
			}

			if (BloodOutlet != null) {
				BloodOutlet.Dispose ();
				BloodOutlet = null;
			}

			if (MarriedDBOutlet != null) {
				MarriedDBOutlet.Dispose ();
				MarriedDBOutlet = null;
			}

			if (MarriageOutlet != null) {
				MarriageOutlet.Dispose ();
				MarriageOutlet = null;
			}

			if (DescendantsOutlet != null) {
				DescendantsOutlet.Dispose ();
				DescendantsOutlet = null;
			}

			if (LinkedOutlet != null) {
				LinkedOutlet.Dispose ();
				LinkedOutlet = null;
			}

			if (UnknownOutlet != null) {
				UnknownOutlet.Dispose ();
				UnknownOutlet = null;
			}
		}
	}
}
