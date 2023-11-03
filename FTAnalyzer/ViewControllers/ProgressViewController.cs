﻿namespace FTAnalyzer.ViewControllers
{
    public partial class ProgressViewController : NSViewController
	{
        internal IProgress<string> ProgressString;
        internal IProgress<int> ProgressValue;

		public ProgressViewController (IntPtr handle) : base (handle)
		{
            ProgressString = new Progress<string>(AppendMessage);
            ProgressValue = new Progress<int>(percent => SetProgress(ProgressBar, percent));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ProgressText.StringValue = string.Empty;
            ProgressBar.DoubleValue = 0;
        }

        void AppendMessage(string message)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => AppendMessage(message));
                return;
            }
            ProgressText.StringValue = message;
        }

        void SetProgress(NSProgressIndicator progressBar, int percent)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => SetProgress(progressBar, percent));
                return;
            }
            ProgressBar.DoubleValue = percent;
        }
    }
}
