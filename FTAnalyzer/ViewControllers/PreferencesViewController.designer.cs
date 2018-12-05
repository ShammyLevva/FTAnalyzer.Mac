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
		AppKit.NSButton AliasinNameDisplayOutlet { get; set; }

		[Outlet]
		AppKit.NSButton AllowEmptyLocationsOutlet { get; set; }

		[Outlet]
		AppKit.NSButton FemaleUnknownOutlet { get; set; }

		[Outlet]
		AppKit.NSButton IgnoreUnknownFactTypeOutlet { get; set; }

		[Outlet]
		AppKit.NSButton LoadWithFiltersOutlet { get; set; }

		[Outlet]
		AppKit.NSPopUpButton LooseBirthMinimumAgeOutlet { get; set; }

		[Outlet]
		AppKit.NSButton RetryFailedLinesOutlet { get; set; }

		[Outlet]
		AppKit.NSButton ShowDuplicateFactsOutlet { get; set; }

		[Outlet]
		AppKit.NSButton ShowMultiAncestorOutlet { get; set; }

		[Outlet]
		AppKit.NSButton ShowWorldEventsOutlet { get; set; }

		[Outlet]
		AppKit.NSButton SkipCensusRefNotesOutlet { get; set; }

		[Outlet]
		AppKit.NSButton UseBaptismDatesOutlet { get; set; }

		[Outlet]
		AppKit.NSButton UseBurialDatesOutlet { get; set; }

		[Outlet]
		AppKit.NSButton UseCountryFirstOutlet { get; set; }
		
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

			if (UseBaptismDatesOutlet != null) {
				UseBaptismDatesOutlet.Dispose ();
				UseBaptismDatesOutlet = null;
			}

			if (UseBurialDatesOutlet != null) {
				UseBurialDatesOutlet.Dispose ();
				UseBurialDatesOutlet = null;
			}

			if (AllowEmptyLocationsOutlet != null) {
				AllowEmptyLocationsOutlet.Dispose ();
				AllowEmptyLocationsOutlet = null;
			}

			if (ShowDuplicateFactsOutlet != null) {
				ShowDuplicateFactsOutlet.Dispose ();
				ShowDuplicateFactsOutlet = null;
			}

			if (LooseBirthMinimumAgeOutlet != null) {
				LooseBirthMinimumAgeOutlet.Dispose ();
				LooseBirthMinimumAgeOutlet = null;
			}

			if (AliasinNameDisplayOutlet != null) {
				AliasinNameDisplayOutlet.Dispose ();
				AliasinNameDisplayOutlet = null;
			}

			if (UseCountryFirstOutlet != null) {
				UseCountryFirstOutlet.Dispose ();
				UseCountryFirstOutlet = null;
			}

			if (ShowWorldEventsOutlet != null) {
				ShowWorldEventsOutlet.Dispose ();
				ShowWorldEventsOutlet = null;
			}

			if (IgnoreUnknownFactTypeOutlet != null) {
				IgnoreUnknownFactTypeOutlet.Dispose ();
				IgnoreUnknownFactTypeOutlet = null;
			}

			if (FemaleUnknownOutlet != null) {
				FemaleUnknownOutlet.Dispose ();
				FemaleUnknownOutlet = null;
			}

			if (ShowMultiAncestorOutlet != null) {
				ShowMultiAncestorOutlet.Dispose ();
				ShowMultiAncestorOutlet = null;
			}

			if (SkipCensusRefNotesOutlet != null) {
				SkipCensusRefNotesOutlet.Dispose ();
				SkipCensusRefNotesOutlet = null;
			}
		}
	}
}
