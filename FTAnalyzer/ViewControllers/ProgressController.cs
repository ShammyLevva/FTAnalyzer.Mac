namespace FTAnalyzer.ViewControllers
{
	public partial class ProgressController : NSWindowController
	{
        public NSViewController Presentor { get; set; }
        ProgressViewController viewController;

		public ProgressController (IntPtr handle) : base (handle)
		{
 		}

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            viewController = ContentViewController as ProgressViewController;
        }

        public string ProgressText
        {
            set => viewController.ProgressString.Report(value);
        }

        public int ProgressBar
        {
            set => viewController.ProgressValue.Report(value);
        }

    }
}
