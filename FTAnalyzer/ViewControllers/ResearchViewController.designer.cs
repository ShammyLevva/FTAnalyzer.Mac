// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
namespace FTAnalyzer.ViewControllers
{
	[Register ("ResearchViewController")]
	partial class ResearchViewController
	{
		[Outlet]
		AppKit.NSButton CanadianColourCensus { get; set; }

		[Outlet]
		AppKit.NSComboBox CensusRegionOutlet { get; set; }

		[Outlet]
		AppKit.NSComboBox CensusSearchProviderOutlet { get; set; }

		[Outlet]
		AppKit.NSButtonCell ColourBMD { get; set; }

		[Outlet]
		AppKit.NSButton IrishColourCensus { get; set; }

		[Outlet]
		AppKit.NSButton UKColourCensus { get; set; }

		[Outlet]
		AppKit.NSButton USColourCensus { get; set; }

		[Action ("CanadianCensusClicked:")]
		partial void CanadianCensusClicked (Foundation.NSObject sender);

		[Action ("CensusRegionChanged:")]
		partial void CensusRegionChanged (Foundation.NSObject sender);

		[Action ("ColourBMDClicked:")]
		partial void ColourBMDClicked (Foundation.NSObject sender);

		[Action ("IrishCensusClicked:")]
		partial void IrishCensusClicked (Foundation.NSObject sender);

		[Action ("SearchProviderChanged:")]
		partial void SearchProviderChanged (Foundation.NSObject sender);

		[Action ("UKCensusClicked:")]
		partial void UKCensusClicked (Foundation.NSObject sender);

		[Action ("USCensusClicked:")]
		partial void USCensusClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CanadianColourCensus != null) {
				CanadianColourCensus.Dispose ();
				CanadianColourCensus = null;
			}

			if (CensusRegionOutlet != null) {
				CensusRegionOutlet.Dispose ();
				CensusRegionOutlet = null;
			}

			if (CensusSearchProviderOutlet != null) {
				CensusSearchProviderOutlet.Dispose ();
				CensusSearchProviderOutlet = null;
			}

			if (IrishColourCensus != null) {
				IrishColourCensus.Dispose ();
				IrishColourCensus = null;
			}

			if (UKColourCensus != null) {
				UKColourCensus.Dispose ();
				UKColourCensus = null;
			}

			if (USColourCensus != null) {
				USColourCensus.Dispose ();
				USColourCensus = null;
			}

			if (ColourBMD != null) {
				ColourBMD.Dispose ();
				ColourBMD = null;
			}
		}
	}
}
