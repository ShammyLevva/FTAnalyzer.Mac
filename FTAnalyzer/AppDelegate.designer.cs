// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace FTAnalyzer
{
	partial class AppDelegate
	{
        [Outlet]
        AppKit.NSMenuItem ExportCustomFactsMenu { get; set; }

        [Outlet]
		AppKit.NSMenuItem ExportDataErrorsMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem ExportDNAGedcomMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem ExportFactsMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem ExportFamiliesMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem ExportIndividualsMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem ExportLocationsMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem ExportLooseBirthsMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem ExportLooseDeathsMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem ExportSourcesMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem PageSetupMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem PrintMenu { get; set; }

        [Action("ExportCustomFacts:")]
        partial void ExportCustomFacts(Foundation.NSObject sender);

		[Action ("ExportDataErrors:")]
		partial void ExportDataErrors (Foundation.NSObject sender);

		[Action ("ExportDNAGedcom:")]
		partial void ExportDNAGedcom (Foundation.NSObject sender);

		[Action ("ExportFacts:")]
		partial void ExportFacts (Foundation.NSObject sender);

		[Action ("ExportFamilies:")]
		partial void ExportFamilies (Foundation.NSObject sender);

		[Action ("ExportIndividuals:")]
		partial void ExportIndividuals (Foundation.NSObject sender);

		[Action ("ExportLocations:")]
		partial void ExportLocations (Foundation.NSObject sender);

		[Action ("ExportLooseBirths:")]
		partial void ExportLooseBirths (Foundation.NSObject sender);

		[Action ("ExportLooseDeaths:")]
		partial void ExportLooseDeaths (Foundation.NSObject sender);

		[Action ("ExportSources:")]
		partial void ExportSources (Foundation.NSObject sender);

		[Action ("PrintClicked:")]
		partial void PrintClicked (Foundation.NSObject sender);

		[Action ("ReportIssue:")]
		partial void ReportIssue (Foundation.NSObject sender);

		[Action ("ViewOnlineGuides:")]
		partial void ViewOnlineGuides (Foundation.NSObject sender);

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
            if (ExportCustomFactsMenu != null)
            {
                ExportCustomFactsMenu.Dispose();
                ExportCustomFactsMenu = null;
            }

			if (ExportDataErrorsMenu != null) {
				ExportDataErrorsMenu.Dispose ();
				ExportDataErrorsMenu = null;
			}

			if (ExportDNAGedcomMenu != null) {
				ExportDNAGedcomMenu.Dispose ();
				ExportDNAGedcomMenu = null;
			}

			if (ExportFactsMenu != null) {
				ExportFactsMenu.Dispose ();
				ExportFactsMenu = null;
			}

			if (ExportFamiliesMenu != null) {
				ExportFamiliesMenu.Dispose ();
				ExportFamiliesMenu = null;
			}

			if (ExportIndividualsMenu != null) {
				ExportIndividualsMenu.Dispose ();
				ExportIndividualsMenu = null;
			}

			if (ExportLocationsMenu != null) {
				ExportLocationsMenu.Dispose ();
				ExportLocationsMenu = null;
			}

			if (ExportLooseBirthsMenu != null) {
				ExportLooseBirthsMenu.Dispose ();
				ExportLooseBirthsMenu = null;
			}

			if (ExportLooseDeathsMenu != null) {
				ExportLooseDeathsMenu.Dispose ();
				ExportLooseDeathsMenu = null;
			}

			if (ExportSourcesMenu != null) {
				ExportSourcesMenu.Dispose ();
				ExportSourcesMenu = null;
			}

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
