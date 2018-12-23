using System;
using System.Collections.Generic;
using AppKit;
using FTAnalyzer.Utilities;
using static FTAnalyzer.ColourValues;

namespace FTAnalyzer.DataSources
{
    public class ColourCensusSource : BindingListTableSource<IDisplayColourCensus>
    {
        readonly Dictionary<int, NSColor> styles;
        string Country { get; }
        int StartIndex { get; }
        int EndIndex { get; }
        string CensusProvider { get; }
        string SearchTooltip { get; }

        public ColourCensusSource(string country, int startIndex, int endIndex, string censusProvider, SortableBindingList<IDisplayColourCensus> reportList)
            : base(reportList)
        {
            Country = country;
            StartIndex = startIndex;
            EndIndex = endIndex;
            CensusProvider = censusProvider;
            SearchTooltip = $"Double click to search {CensusProvider}.";

            styles = new Dictionary<int, NSColor>();
            NSColor notAlive = Color.DarkGray;
            styles.Add(0, notAlive);
            NSColor missingCensus = Color.Red;
            styles.Add(1, missingCensus);
            NSColor censusMissingLC = Color.Yellow;
            styles.Add(2, censusMissingLC);
            NSColor notCensusEnterednotLCYear = Color.LawnGreen;
            styles.Add(3, notCensusEnterednotLCYear);
            NSColor allEntered = Color.LawnGreen;
            styles.Add(4, allEntered);
            NSColor lcNoCensus = Color.DarkOrange;
            styles.Add(5, lcNoCensus);
            NSColor onOtherCensus = Color.DarkSlateGray;
            styles.Add(6, onOtherCensus);
            NSColor outsideUKCensus = Color.DarkSlateGray;
            styles.Add(7, onOtherCensus);
            NSColor knownMissing = Color.MediumSeaGreen;
            styles.Add(8, knownMissing);
        }

        NSTextField GetTextField(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            if (index < 0 || index > _properties.Length)
                return null;
            if (index >= StartIndex && index <= EndIndex)
                Console.WriteLine("Colour Column.");
            var property = _properties[index];
            NSTextAlignment alignment = NSTextAlignment.Left;
            ColumnDetail[] x = property.GetCustomAttributes(typeof(ColumnDetail), false) as ColumnDetail[];
            if (x?.Length == 1)
                alignment = x[0].Alignment;

            if (!(tableView.MakeView(CellIdentifier, this) is NSTextField view))
            {
                view = new NSTextField
                {
                    Identifier = CellIdentifier,
                    BackgroundColor = NSColor.Clear,
                    LineBreakMode = NSLineBreakMode.TruncatingTail,
                    Bordered = false,
                    Selectable = false,
                    Editable = false,
                    Alignment = alignment,
                    AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable,
                    TranslatesAutoresizingMaskIntoConstraints = false
                };
                if (tableView.AutosaveName == "PrintView")
                    view.Font = NSFont.SystemFontOfSize(8);
            }
            // Setup view based on the column selected
            if (row >= 0)
            {
                var item = _bindingList[(int)row];
                var propertyValue = _properties[index].GetValue(item);
                if (index >= StartIndex && index <= EndIndex)
                    view.IntValue = (int)propertyValue;
                else
                    view.StringValue = propertyValue == null ? string.Empty : propertyValue.ToString();
            }
            else
                view.StringValue = string.Empty;
            return view;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            NSTextField textField = GetTextField(tableView, tableColumn, row) as NSTextField;
            
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            var c1939index = Array.IndexOf(_fieldNames, "C1939");
            if (index >= StartIndex && index <= EndIndex)
            {
                switch (textField.IntValue)
                {
                    case (int)CensusColour.NOT_ALIVE:
                        textField.BackgroundColor = styles[(int)CensusColour.NOT_ALIVE];
                        textField.TextColor = styles[(int)CensusColour.NOT_ALIVE];
                        textField.ToolTip = "Not alive at time of census. {SearchTooltip}";
                        break;
                    case (int)CensusColour.NO_CENSUS:
                        textField.BackgroundColor = styles[(int)CensusColour.NO_CENSUS];
                        textField.TextColor = styles[(int)CensusColour.NO_CENSUS];
                        textField.ToolTip = index == c1939index
                            ? CensusProvider.Equals("Find My Past")
                            ? $"No census information entered. {SearchTooltip}"
                                : $"No census information entered. No search on {CensusProvider} available."
                            : $"No census information entered. {SearchTooltip}";
                        break;
                    case (int)CensusColour.CENSUS_PRESENT_LC_MISSING:
                        textField.BackgroundColor = styles[(int)CensusColour.CENSUS_PRESENT_LC_MISSING];
                        textField.TextColor = styles[(int)CensusColour.CENSUS_PRESENT_LC_MISSING];
                        textField.ToolTip = "Census entered but no Lost Cousins flag set.";
                        break;
                    case (int)CensusColour.CENSUS_PRESENT_NOT_LC_YEAR:
                        textField.BackgroundColor = styles[(int)CensusColour.CENSUS_PRESENT_NOT_LC_YEAR];
                        textField.TextColor = styles[(int)CensusColour.CENSUS_PRESENT_NOT_LC_YEAR];
                        textField.ToolTip = "Census entered and not a Lost Cousins year.";
                        break;
                    case (int)CensusColour.CENSUS_PRESENT_LC_PRESENT:
                        textField.BackgroundColor = styles[(int)CensusColour.CENSUS_PRESENT_LC_PRESENT];
                        textField.TextColor = styles[(int)CensusColour.CENSUS_PRESENT_LC_PRESENT];
                        textField.ToolTip = "Census entered and flagged as entered on Lost Cousins.";
                        break;
                    case (int)CensusColour.LC_PRESENT_NO_CENSUS:
                        textField.BackgroundColor = styles[(int)CensusColour.LC_PRESENT_NO_CENSUS];
                        textField.TextColor = styles[(int)CensusColour.LC_PRESENT_NO_CENSUS];
                        textField.ToolTip = "Lost Cousins flagged but no Census entered. {SearchTooltip}";
                        break;
                    case (int)CensusColour.OVERSEAS_CENSUS:
                        textField.BackgroundColor = styles[(int)CensusColour.OVERSEAS_CENSUS];
                        textField.TextColor = styles[(int)CensusColour.OVERSEAS_CENSUS];
                        textField.ToolTip = $"On Census outside {Country}.";
                        break;
                    case (int)CensusColour.OUT_OF_COUNTRY:
                        textField.BackgroundColor = styles[(int)CensusColour.OUT_OF_COUNTRY];
                        textField.TextColor = styles[(int)CensusColour.OUT_OF_COUNTRY];
                        textField.ToolTip = $"Likely outside {Country} on census date. {SearchTooltip}";
                        break;
                    case (int)CensusColour.KNOWN_MISSING:
                        textField.BackgroundColor = styles[(int)CensusColour.KNOWN_MISSING];
                        textField.TextColor = styles[(int)CensusColour.KNOWN_MISSING];
                        textField.ToolTip = "Known to be missing from the census.";
                        break;
                }
            }
            return textField;
        }
    }
}