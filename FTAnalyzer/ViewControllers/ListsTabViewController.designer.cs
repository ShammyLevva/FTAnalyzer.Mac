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
	[Register ("ListsTabViewController")]
	partial class ListsTabViewController
	{
		[Outlet]
		AppKit.NSMenuItem SetRootPersonMenu { get; set; }

		[Action ("SetRootPersonClicked:")]
		partial void SetRootPersonClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (SetRootPersonMenu != null) {
				SetRootPersonMenu.Dispose ();
				SetRootPersonMenu = null;
			}
		}
	}
}
