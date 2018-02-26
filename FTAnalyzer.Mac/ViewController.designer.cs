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
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSProgressIndicator _familiesProgress { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator _individualsProgress { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator _relationshipsProgress { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator _sourcesProgress { get; set; }

		[Outlet]
		AppKit.NSTextView _statusTextView { get; set; }

		[Outlet]
		AppKit.NSTextField _titleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_statusTextView != null) {
				_statusTextView.Dispose ();
				_statusTextView = null;
			}

			if (_sourcesProgress != null) {
				_sourcesProgress.Dispose ();
				_sourcesProgress = null;
			}

			if (_individualsProgress != null) {
				_individualsProgress.Dispose ();
				_individualsProgress = null;
			}

			if (_familiesProgress != null) {
				_familiesProgress.Dispose ();
				_familiesProgress = null;
			}

			if (_relationshipsProgress != null) {
				_relationshipsProgress.Dispose ();
				_relationshipsProgress = null;
			}

			if (_titleLabel != null) {
				_titleLabel.Dispose ();
				_titleLabel = null;
			}
		}
	}
}
