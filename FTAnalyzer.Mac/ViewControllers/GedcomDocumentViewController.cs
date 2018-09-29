using System;

using AppKit;
using Foundation;

namespace FTAnalyzer.Mac
{
    public partial class GedcomDocumentViewController : NSViewController
    {
        IProgress<string> _messages;
        IProgress<int> _sources;
        IProgress<int> _individuals;
        IProgress<int> _families;
        IProgress<int> _relationships;
		GedcomDocument document;
 
        public GedcomDocumentViewController(IntPtr handle) : base(handle)
        {
            _messages = new Progress<string>(message => AppendMessage(message));
            _sources = new Progress<int>(percent => SetProgress(_sourcesProgress, percent));
            _individuals = new Progress<int>(percent => SetProgress(_individualsProgress, percent));
            _families = new Progress<int>(percent => SetProgress(_familiesProgress, percent));
            _relationships = new Progress<int>(percent => SetProgress(_relationshipsProgress, percent));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			// Do any additional setup after loading the view.
			ThisApp.Document = document;
            var font = NSFont.FromFontName("Kunstler Script", 72.0f);
            _titleLabel.Font = font;
        }

		#region Computed Properties
        /// <summary>
        /// Returns the delegate of the current running application
        /// </summary>
        /// <value>The this app.</value>
        public AppDelegate ThisApp
        {
            get { return (AppDelegate)NSApplication.SharedApplication.Delegate; }
        }
        #endregion

        public override NSObject RepresentedObject => base.RepresentedObject;

        public IProgress<string> Messages => _messages;
        public IProgress<int> Sources => _sources;
        public IProgress<int> Individuals => _individuals;
        public IProgress<int> Families => _families;
        public IProgress<int> Relationships => _relationships;

        public void ClearAllProgress()
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(ClearAllProgress);
                return;
            }

            _statusTextView.Value = string.Empty;
            _sourcesProgress.DoubleValue = 0;
            _individualsProgress.DoubleValue = 0;
            _familiesProgress.DoubleValue = 0;
            _relationshipsProgress.DoubleValue = 0;
        }

        void AppendMessage(string message)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => AppendMessage(message));
                return;
            }

            if (_statusTextView.Value == null)
            {
                _statusTextView.Value = message;
            }
            else
            {
                _statusTextView.Value += message;
            }
        }

        void SetProgress(NSProgressIndicator progressBar, int percent)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => SetProgress(progressBar, percent));
                return;
            }

            progressBar.DoubleValue = percent;
        }
    }
}
