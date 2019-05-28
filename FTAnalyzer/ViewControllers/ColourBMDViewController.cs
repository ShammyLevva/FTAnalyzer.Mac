using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace FTAnalyzer.ViewControllers
{
    public partial class ColourBMDViewController : BindingListViewController<IDisplayColourBMD>
    {
        #region Constructors

        // Called when created from unmanaged code
        public ColourBMDViewController(IntPtr handle) : base(string.Empty,string.Empty)
        { 
        }

        // Call to load from the XIB/NIB file
        public ColourBMDViewController() : base("BMD Research Suggestions", string.Empty)
        {
        }

        #endregion
    }
}
