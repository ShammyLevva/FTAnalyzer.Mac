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
	[Register ("LCReportsViewController")]
	partial class LCReportsViewController
	{
		[Outlet]
		AppKit.NSScrollView ReportsTextbox { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ReportsTextbox != null) {
				ReportsTextbox.Dispose ();
				ReportsTextbox = null;
			}
		}
	}
}
