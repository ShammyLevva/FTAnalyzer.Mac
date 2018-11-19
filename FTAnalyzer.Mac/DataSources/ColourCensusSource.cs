using System;
using System.Collections.Generic;
using AppKit;
using FTAnalyzer.Utilities;
namespace FTAnalyzer.Mac.DataSources
{
    public class ColourCensusSource : BindingListTableSource<IDisplayColourCensus>
    {
        Dictionary<int, NSCellType> styles;
        public string Country { get; }

        public ColourCensusSource(string country, SortableBindingList<IDisplayColourCensus> reportList)
            : base(reportList)
        {
            Country = country;
      
            styles = new Dictionary<int, NSCellType>();
            //NSCellType notAlive = new NSCellType();
            //notAlive = notAlive.ForeColor = Color.DarkGray;
            //styles.Add(0, notAlive);
            //NSCellType missingCensus = new NSCellType();
            //missingCensus.BackColor = missingCensus.ForeColor = Color.Red;
            //styles.Add(1, missingCensus);
            //NSCellType censusMissingLC = new NSCellType();
            //censusMissingLC.BackColor = censusMissingLC.ForeColor = Color.Yellow;
            //styles.Add(2, censusMissingLC);
            //NSCellType notCensusEnterednotLCYear = new NSCellType();
            //notCensusEnterednotLCYear.BackColor = notCensusEnterednotLCYear.ForeColor = Color.LawnGreen;
            //styles.Add(3, notCensusEnterednotLCYear);
            //NSCellType allEntered = new NSCellType();
            //allEntered.BackColor = allEntered.ForeColor = Color.LawnGreen;
            //styles.Add(4, allEntered);
            //NSCellType lcNoCensus = new NSCellType();
            //lcNoCensus.BackColor = lcNoCensus.ForeColor = Color.DarkOrange;
            //styles.Add(5, lcNoCensus);
            //NSCellType onOtherCensus = new NSCellType();
            //onOtherCensus.BackColor = onOtherCensus.ForeColor = Color.DarkSlateGray;
            //styles.Add(6, onOtherCensus);
            //NSCellType outsideUKCensus = new NSCellType();
            //outsideUKCensus.BackColor = outsideUKCensus.ForeColor = Color.DarkSlateGray;
            //styles.Add(7, onOtherCensus);
            //NSCellType knownMissing = new NSCellType();
            //knownMissing.BackColor = knownMissing.ForeColor = NSColor.MediumSeaGreen;
            //styles.Add(8, knownMissing);
            //SetColumns();
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            NSTextView textView = GetFTAnalyzerGridCell(tableView,tableColumn, row) as NSTextView;
            //if(textView != null)
                //textView.BackgroundColor = Color.DarkOrange;
            return textView;
        }
    }
}
