using System;

using AppKit;
using Foundation;

namespace FTAnalyzer.Mac
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
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

        public int SourcesProgressValue
        {
            get { return (int)SourcesProgress.DoubleValue; }
            set { SourcesProgress.DoubleValue = value; }
        }

        public int IndividualsProgressValue
        {
            get { return (int)IndividualsProgress.DoubleValue; }
            set { IndividualsProgress.DoubleValue = value; }
        }

        public int FamiliesProgressValue
        {
            get { return (int)FamiliesProgress.DoubleValue; }
            set { FamiliesProgress.DoubleValue = value; }
        }

        public int RelationshipsProgressValue
        {
            get { return (int)RelationshipsProgress.DoubleValue; }
            set { RelationshipsProgress.DoubleValue = value; }
        }

        public string StatusText
        {
            get { return StatusTextView.Value; }
            set { StatusTextView.Value = value; }
        }
    }
}
