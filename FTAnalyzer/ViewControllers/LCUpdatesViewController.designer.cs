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
	[Register ("LCUpdatesViewController")]
	partial class LCUpdatesViewController
	{
		[Outlet]
		AppKit.NSButton ConfirmRootPerson { get; set; }

		[Outlet]
		AppKit.NSTextField EmailAddressField { get; set; }

		[Outlet]
		AppKit.NSButton LostCousinsUpdateButton { get; set; }

		[Outlet]
		AppKit.NSScrollView OutputTextbox { get; set; }

		[Outlet]
		AppKit.NSSecureTextField PasswordField { get; set; }

		[Outlet]
		AppKit.NSScrollView UpdateResultTextbox { get; set; }

		[Action ("ConfirmRootPersonChecked:")]
		partial void ConfirmRootPersonChecked (Foundation.NSObject sender);

		[Action ("LostCousinsUpdateClicked:")]
		partial void LostCousinsUpdateClicked (Foundation.NSObject sender);

		[Action ("UpdateWebsiteClicked:")]
		partial void UpdateWebsiteClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (OutputTextbox != null) {
				OutputTextbox.Dispose ();
				OutputTextbox = null;
			}

			if (UpdateResultTextbox != null) {
				UpdateResultTextbox.Dispose ();
				UpdateResultTextbox = null;
			}

			if (LostCousinsUpdateButton != null) {
				LostCousinsUpdateButton.Dispose ();
				LostCousinsUpdateButton = null;
			}

			if (ConfirmRootPerson != null) {
				ConfirmRootPerson.Dispose ();
				ConfirmRootPerson = null;
			}

			if (EmailAddressField != null) {
				EmailAddressField.Dispose ();
				EmailAddressField = null;
			}

			if (PasswordField != null) {
				PasswordField.Dispose ();
				PasswordField = null;
			}
		}
	}
}
