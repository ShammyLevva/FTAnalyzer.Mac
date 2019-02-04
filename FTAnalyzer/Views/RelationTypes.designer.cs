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
	[Register ("RelationTypes")]
	partial class RelationTypes
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

		[Action ("BloodChecked:")]
		partial void BloodChecked (Foundation.NSObject sender);

		[Action ("ByMarriageChecked:")]
		partial void ByMarriageChecked (Foundation.NSObject sender);

		[Action ("DescendantsChecked:")]
		partial void DescendantsChecked (Foundation.NSObject sender);

		[Action ("DirectChecked:")]
		partial void DirectChecked (Foundation.NSObject sender);

		[Action ("LinkedChecked:")]
		partial void LinkedChecked (Foundation.NSObject sender);

		[Action ("MarriedDBChecked:")]
		partial void MarriedDBChecked (Foundation.NSObject sender);

		[Action ("UnknownChecked:")]
		partial void UnknownChecked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BloodOutlet != null) {
				BloodOutlet.Dispose ();
				BloodOutlet = null;
			}

			if (DescendantsOutlet != null) {
				DescendantsOutlet.Dispose ();
				DescendantsOutlet = null;
			}

			if (DirectOutlet != null) {
				DirectOutlet.Dispose ();
				DirectOutlet = null;
			}

			if (LinkedOutlet != null) {
				LinkedOutlet.Dispose ();
				LinkedOutlet = null;
			}

			if (MarriageOutlet != null) {
				MarriageOutlet.Dispose ();
				MarriageOutlet = null;
			}

			if (MarriedDBOutlet != null) {
				MarriedDBOutlet.Dispose ();
				MarriedDBOutlet = null;
			}

			if (UnknownOutlet != null) {
				UnknownOutlet.Dispose ();
				UnknownOutlet = null;
			}
		}
	}
}
