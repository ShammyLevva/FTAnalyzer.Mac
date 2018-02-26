using System;

using AppKit;
using Foundation;

namespace FTAnalyzer.Mac
{
    public partial class ViewController : NSViewController
    {
        IProgress<string> _messages;
        IProgress<int> _sources;
        IProgress<int> _individuals;
        IProgress<int> _families;
        IProgress<int> _relationships;

        public ViewController(IntPtr handle) : base(handle)
        {
            _messages = new Progress<string>(AppendMessage);
            _sources = new Progress<int>(percent => SetProgress(_sourcesProgress, percent));
            _individuals = new Progress<int>(percent => SetProgress(_individualsProgress, percent));
            _families = new Progress<int>(percent => SetProgress(_familiesProgress, percent));
            _relationships = new Progress<int>(percent => SetProgress(_relationshipsProgress, percent));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.

            var font = NSFont.FromFontName("Kunstler Script", 52.0f);
            _titleLabel.Font = font;
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

        public IProgress<string> Messages
        {
            get { return _messages; }
        }

        public IProgress<int> Sources
        {
            get { return _sources; }
        }

        public IProgress<int> Individuals
        {
            get { return _individuals; }
        }

        public IProgress<int> Families
        {
            get { return _families; }
        }

        public IProgress<int> Relationships
        {
            get { return _relationships; }
        }

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
