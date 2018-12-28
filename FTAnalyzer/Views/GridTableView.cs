using System;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace FTAnalyzer.Views
{
    public class GridTableView : NSTableView
    {
        public GridTableView(string title, NSObject target)
        {
            Identifier = title;
            RowSizeStyle = NSTableViewRowSizeStyle.Default;
            Enabled = true;
            UsesAlternatingRowBackgroundColors = true;
            AutoresizesSubviews = true;
            ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.None;
            WantsLayer = true;
            Layer = new CALayer { Bounds = new CGRect(0, 0, 0, 0) };
            Bounds = new CGRect(0, 0, 0, 0);
            AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
            AllowsMultipleSelection = false;
            AllowsColumnResizing = true;
            AllowsColumnSelection = false;
            AllowsColumnReordering = false;
            SortDescriptors = new NSSortDescriptor[] { };
            AutosaveName = title;
            AutosaveTableColumns = true;
            Target = target;
            DoubleAction = new ObjCRuntime.Selector("ViewDetailsSelector");
        }

        public override NSMenu MenuForEvent(NSEvent theEvent)
        {
            if(Identifier == "Individuals")
               return base.MenuForEvent(theEvent);
            var emptyMenu = new NSMenu();
            emptyMenu.PopUpMenu(null, new CGPoint(0, 0), this); 
            return emptyMenu;

        }
    }
}
