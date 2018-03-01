// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace FTAnalyzer.Mac
{
	[Register ("IndividualsController")]
	partial class IndividualsController
	{
		[Outlet]
		AppKit.NSTableView IndividualsTableVIew { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (IndividualsTableVIew != null) {
				IndividualsTableVIew.Dispose ();
				IndividualsTableVIew = null;
			}
		}
	}
}
