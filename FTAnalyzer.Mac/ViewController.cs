using System;

using AppKit;
using Foundation;

namespace FTAnalyzer.Mac
{
    public partial class ViewController : NSViewController
    {
        GedcomDocument _document;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.

            var font = NSFont.FromFontName("Kunstler Script", 52.0f);
            TitleLabel.Font = font;
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }

        [Export("Document")]
        public GedcomDocument Document
        {
            get { return _document; }
            set
            {
                WillChangeValue("Document");
                _document = value;
                DidChangeValue("Document");
            }
        }
    }
}
