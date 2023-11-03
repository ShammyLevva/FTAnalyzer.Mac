// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
namespace FTAnalyzer.ViewControllers
{
	[Register ("LCUpdatesViewController")]
	partial class LCUpdatesViewController
	{
		[Outlet]
		AppKit.NSButton ConfirmRootPerson { get; set; }

		[Outlet]
		AppKit.NSTextField EmailAddressField { get; set; }

		[Outlet]
		AppKit.NSScrollView LCScrollingTextOutlet { get; set; }

		[Outlet]
		AppKit.NSButton LoginButtonOutlet { get; set; }

		[Outlet]
		AppKit.NSButton LostCousinsUpdateButton { get; set; }

		[Outlet]
		AppKit.NSSecureTextField PasswordField { get; set; }

		[Outlet]
		AppKit.NSTextView StatsTextbox { get; set; }

		[Outlet]
		AppKit.NSTextView UpdateResultsTextbox { get; set; }

		[Outlet]
		AppKit.NSButton UseKeychainOutlet { get; set; }

		[Action ("ConfirmRootPersonChecked:")]
		partial void ConfirmRootPersonChecked (Foundation.NSObject sender);

		[Action ("LoginButtonClicked:")]
		partial void LoginButtonClicked (Foundation.NSObject sender);

		[Action ("LostCousinsUpdateClicked:")]
		partial void LostCousinsUpdateClicked (Foundation.NSObject sender);

		[Action ("UseKeychainChecked:")]
		partial void UseKeychainChecked (Foundation.NSObject sender);

		[Action ("ViewInvalidClicked:")]
		partial void ViewInvalidClicked (Foundation.NSObject sender);

		[Action ("ViewPotentialClicked:")]
		partial void ViewPotentialClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (LCScrollingTextOutlet != null) {
				LCScrollingTextOutlet.Dispose ();
				LCScrollingTextOutlet = null;
			}

			if (ConfirmRootPerson != null) {
				ConfirmRootPerson.Dispose ();
				ConfirmRootPerson = null;
			}

			if (EmailAddressField != null) {
				EmailAddressField.Dispose ();
				EmailAddressField = null;
			}

			if (LoginButtonOutlet != null) {
				LoginButtonOutlet.Dispose ();
				LoginButtonOutlet = null;
			}

			if (LostCousinsUpdateButton != null) {
				LostCousinsUpdateButton.Dispose ();
				LostCousinsUpdateButton = null;
			}

			if (PasswordField != null) {
				PasswordField.Dispose ();
				PasswordField = null;
			}

			if (StatsTextbox != null) {
				StatsTextbox.Dispose ();
				StatsTextbox = null;
			}

			if (UpdateResultsTextbox != null) {
				UpdateResultsTextbox.Dispose ();
				UpdateResultsTextbox = null;
			}

			if (UseKeychainOutlet != null) {
				UseKeychainOutlet.Dispose ();
				UseKeychainOutlet = null;
			}
		}
	}
}
