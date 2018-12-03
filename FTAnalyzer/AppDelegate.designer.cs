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
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSMenuItem PageSetupMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem PrintMenu { get; set; }

		[Action ("Options:")]
		partial void Options (Foundation.NSObject sender);

		[Action ("ReportIssue:")]
		partial void ReportIssue (Foundation.NSObject sender);

		[Action ("ViewOnlineGUides:")]
		partial void ViewOnlineGUides (Foundation.NSObject sender);

		[Action ("ViewOnlineManual:")]
		partial void ViewOnlineManual (Foundation.NSObject sender);

		[Action ("VisitFacebookSupport:")]
		partial void VisitFacebookSupport (Foundation.NSObject sender);

		[Action ("VisitFacebookUserGroup:")]
		partial void VisitFacebookUserGroup (Foundation.NSObject sender);

		[Action ("VisitPrivacyPolicy:")]
		partial void VisitPrivacyPolicy (Foundation.NSObject sender);

		[Action ("VisitWhatsNew:")]
		partial void VisitWhatsNew (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (PageSetupMenu != null) {
				PageSetupMenu.Dispose ();
				PageSetupMenu = null;
			}

			if (PrintMenu != null) {
				PrintMenu.Dispose ();
				PrintMenu = null;
			}
		}
	}
}
