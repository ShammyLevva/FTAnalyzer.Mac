using System;
using System.Collections.Generic;
using AppKit;
using FTAnalyzer.Utilities;
using FTAnalyzer.ViewControllers;
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

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            if ((index >= ColourCensusViewController.CensusColumnsStart && index < StartIndex) ||
                (index > EndIndex && index <= ColourCensusViewController.CensusColumnsEnd)) // if we are asking for columns that aren't part of this country's Census return null
                return null;
            NSTableCellView cellView = GetFTAnalyzerGridCell(tableView, tableColumn, row);
            SetTextField(cellView.TextField, tableColumn, row);
            var textField = cellView.TextField;
            
            var c1939index = Array.IndexOf(_fieldNames, "C1939");
            if (index >= StartIndex && index <= EndIndex)
            {
                switch (textField.IntValue)
                {
                    case (int)CensusColours.NOT_ALIVE:
                        textField.BackgroundColor = styles[(int)CensusColours.NOT_ALIVE];
                        textField.TextColor = styles[(int)CensusColours.NOT_ALIVE];
                        textField.ToolTip = "Not alive at time of census. {SearchTooltip}";
                        break;
                    case (int)CensusColours.NO_CENSUS:
                        textField.BackgroundColor = styles[(int)CensusColours.NO_CENSUS];
                        textField.TextColor = styles[(int)CensusColours.NO_CENSUS];
                        textField.ToolTip = index == c1939index
                            ? CensusProvider.Equals("Find My Past")
                                ? $"No National Register information entered. {SearchTooltip}"
                                : $"No National Register information entered. No search on {CensusProvider} available."
                            : $"No census information entered. {SearchTooltip}";
                        break;
                    case (int)CensusColours.CENSUS_PRESENT_LC_MISSING:
                        textField.BackgroundColor = styles[(int)CensusColours.CENSUS_PRESENT_LC_MISSING];
                        textField.TextColor = styles[(int)CensusColours.CENSUS_PRESENT_LC_MISSING];
                        textField.ToolTip = "Census entered but no Lost Cousins flag set.";
                        break;
                    case (int)CensusColours.CENSUS_PRESENT_NOT_LC_YEAR:
                        textField.BackgroundColor = styles[(int)CensusColours.CENSUS_PRESENT_NOT_LC_YEAR];
                        textField.TextColor = styles[(int)CensusColours.CENSUS_PRESENT_NOT_LC_YEAR];
                        textField.ToolTip = "Census entered and not a Lost Cousins year.";
                        break;
                    case (int)CensusColours.CENSUS_PRESENT_LC_PRESENT:
                        textField.BackgroundColor = styles[(int)CensusColours.CENSUS_PRESENT_LC_PRESENT];
                        textField.TextColor = styles[(int)CensusColours.CENSUS_PRESENT_LC_PRESENT];
                        textField.ToolTip = "Census entered and flagged as entered on Lost Cousins.";
                        break;
                    case (int)CensusColours.LC_PRESENT_NO_CENSUS:
                        textField.BackgroundColor = styles[(int)CensusColours.LC_PRESENT_NO_CENSUS];
                        textField.TextColor = styles[(int)CensusColours.LC_PRESENT_NO_CENSUS];
                        textField.ToolTip = "Lost Cousins flagged but no Census entered. {SearchTooltip}";
                        break;
                    case (int)CensusColours.OVERSEAS_CENSUS:
                        textField.BackgroundColor = styles[(int)CensusColours.OVERSEAS_CENSUS];
                        textField.TextColor = styles[(int)CensusColours.OVERSEAS_CENSUS];
                        textField.ToolTip = $"On Census outside {Country}.";
                        break;
                    case (int)CensusColours.OUT_OF_COUNTRY:
                        textField.BackgroundColor = styles[(int)CensusColours.OUT_OF_COUNTRY];
                        textField.TextColor = styles[(int)CensusColours.OUT_OF_COUNTRY];
                        textField.ToolTip = $"Likely outside {Country} on census date. {SearchTooltip}";
                        break;
                    case (int)CensusColours.KNOWN_MISSING:
                        textField.BackgroundColor = styles[(int)CensusColours.KNOWN_MISSING];
                        textField.TextColor = styles[(int)CensusColours.KNOWN_MISSING];
                        textField.ToolTip = "Known to be missing from the census.";
                        break;
                    default:
                        break;
                }
            }
            return cellView;
        }

        void SetTextField(NSTextField view, NSTableColumn tableColumn, nint row)
        {
            // Setup view based on the column selected
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            if (row >= 0)
            {
                var item = _bindingList[(int)row];
                var propertyValue = _properties[index].GetValue(item);
                if (index >= ColourCensusViewController.CensusColumnsStart && index <= ColourCensusViewController.CensusColumnsEnd)
                    view.IntValue = (int)propertyValue;
                else
                    view.StringValue = propertyValue == null ? string.Empty : propertyValue.ToString();
            }
            else
                view.StringValue = string.Empty;
        }
    }
}