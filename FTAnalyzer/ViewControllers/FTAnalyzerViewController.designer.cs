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
	[Register ("FTAnalyzerTabViewController")]
	partial class FTAnalyzerViewController
	{
		[Outlet]
		AppKit.NSTabViewItem ResearchSuggestions { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ResearchSuggestions != null) {
				ResearchSuggestions.Dispose ();
				ResearchSuggestions = null;
			}
		}
	}
}
