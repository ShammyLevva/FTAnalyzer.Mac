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
	[Register ("ResearchViewController")]
	partial class ResearchViewController
	{
		[Outlet]
		AppKit.NSButton CanadianColourCensus { get; set; }

		[Outlet]
		AppKit.NSButton IrishColourCensus { get; set; }

		[Outlet]
		AppKit.NSButton UKColourCensus { get; set; }

		[Outlet]
		AppKit.NSButton USColourCensus { get; set; }

		[Action ("CanadianCensusClicked:")]
		partial void CanadianCensusClicked (Foundation.NSObject sender);

		[Action ("IrishCensusClicked:")]
		partial void IrishCensusClicked (Foundation.NSObject sender);

		[Action ("UKCensusClicked:")]
		partial void UKCensusClicked (Foundation.NSObject sender);

		[Action ("USCensusClicked:")]
		partial void USCensusClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (UKColourCensus != null) {
				UKColourCensus.Dispose ();
				UKColourCensus = null;
			}

			if (IrishColourCensus != null) {
				IrishColourCensus.Dispose ();
				IrishColourCensus = null;
			}

			if (USColourCensus != null) {
				USColourCensus.Dispose ();
				USColourCensus = null;
			}

			if (CanadianColourCensus != null) {
				CanadianColourCensus.Dispose ();
				CanadianColourCensus = null;
			}
		}
	}
}
