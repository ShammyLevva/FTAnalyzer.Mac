namespace FTAnalyzer.Utilities
{
    [Register("ReplaceViewSeque")]
    public class ReplaceViewSeque : NSStoryboardSegue
    {
        #region Constructors
        public ReplaceViewSeque()
        {
        }

        public ReplaceViewSeque(string identifier, NSObject sourceController, NSObject destinationController) : base(identifier, sourceController, destinationController)
        {
        }

        public ReplaceViewSeque(IntPtr handle) : base(handle)
        {
        }

        public ReplaceViewSeque(NSObjectFlag x) : base(x)
        {
        }
        #endregion

        #region Override Methods
        public override void Perform()
        {
            // Cast the source and destination controllers
            var destination = DestinationController as NSViewController;

            // Is there a source?
            if (!(SourceController is NSViewController source))
            {
                // No, get the current key window
                var window = NSApplication.SharedApplication.KeyWindow;
                window.ContentViewController = destination; // Swap the controllers
                window.ContentViewController?.RemoveFromParentViewController(); // Release memory
            }
            else
            {
                source.View.Window.ContentViewController = destination; // Swap the controllers
                source.RemoveFromParentViewController(); // Release memory
            }
        }
        #endregion

    }
}
