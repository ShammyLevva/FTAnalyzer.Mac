// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace FTAnalyzer.ViewControllers
{
	[Register ("LCReportsViewController")]
	partial class LCReportsViewController
	{
		[Outlet]
		FTAnalyzer.RelationTypes RelationshipTypesOutlet { get; set; }

		[Outlet]
		AppKit.NSTextView ReportsTextBox { get; set; }

		[Outlet]
		AppKit.NSButton ShowAlreadyEnteredOutlet { get; set; }

		[Action ("Canada1880CensusClicked:")]
		partial void Canada1880CensusClicked (Foundation.NSObject sender);

		[Action ("EW1841CensusClicked:")]
		partial void EW1841CensusClicked (Foundation.NSObject sender);

		[Action ("EW1881CensusClicked:")]
		partial void EW1881CensusClicked (Foundation.NSObject sender);

		[Action ("EW1911CensusClicked:")]
		partial void EW1911CensusClicked (Foundation.NSObject sender);

		[Action ("Ireland1911CensusClicked:")]
		partial void Ireland1911CensusClicked (Foundation.NSObject sender);

		[Action ("LostCousinsBadCensusClicked:")]
		partial void LostCousinsBadCensusClicked (Foundation.NSObject sender);

		[Action ("LostCousinsDuplicatesClicked:")]
		partial void LostCousinsDuplicatesClicked (Foundation.NSObject sender);

		[Action ("LostCousinsNoCountryClicked:")]
		partial void LostCousinsNoCountryClicked (Foundation.NSObject sender);

		[Action ("LostCousinsWebsiteClicked:")]
		partial void LostCousinsWebsiteClicked (Foundation.NSObject sender);

		[Action ("Scotland1881CensusClicked:")]
		partial void Scotland1881CensusClicked (Foundation.NSObject sender);

		[Action ("US1880CensusClicked:")]
		partial void US1880CensusClicked (Foundation.NSObject sender);

		[Action ("US1940CensusClicked:")]
		partial void US1940CensusClicked (Foundation.NSObject sender);

		[Action ("VisitLostCousinsForumClicked:")]
		partial void VisitLostCousinsForumClicked (Foundation.NSObject sender);

		[Action ("VisitLostCousinsWebsiteClicked:")]
		partial void VisitLostCousinsWebsiteClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (RelationshipTypesOutlet != null) {
				RelationshipTypesOutlet.Dispose ();
				RelationshipTypesOutlet = null;
			}

			if (ReportsTextBox != null) {
				ReportsTextBox.Dispose ();
				ReportsTextBox = null;
			}

			if (ShowAlreadyEnteredOutlet != null) {
				ShowAlreadyEnteredOutlet.Dispose ();
				ShowAlreadyEnteredOutlet = null;
			}
		}
	}
}
