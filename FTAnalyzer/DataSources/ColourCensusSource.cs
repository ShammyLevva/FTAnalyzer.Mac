﻿using System;
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

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            NSTextField textField = GetFTAnalyzerGridCell(tableView, tableColumn, row) as NSTextField;
            
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            var c1939index = Array.IndexOf(_fieldNames, "C1939");
            if (index >= StartIndex && index <= EndIndex)
            {
                switch (textField.StringValue)
                {
                    case "NOT_ALIVE":
                        textField.BackgroundColor = styles[(int)CensusColour.NOT_ALIVE];
                        textField.TextColor = styles[(int)CensusColour.NOT_ALIVE];
                        textField.ToolTip = "Not alive at time of census. {SearchTooltip}";
                        break;
                    case "NO_CENSUS":
                        textField.BackgroundColor = styles[(int)CensusColour.NO_CENSUS];
                        textField.TextColor = styles[(int)CensusColour.NO_CENSUS];
                        textField.ToolTip = index == c1939index
                            ? CensusProvider.Equals("Find My Past")
                            ? $"No census information entered. {SearchTooltip}"
                                : $"No census information entered. No search on {CensusProvider} available."
                            : $"No census information entered. {SearchTooltip}";
                        break;
                    case "CENSUS_PRESENT_LC_MISSING":
                        textField.BackgroundColor = styles[(int)CensusColour.CENSUS_PRESENT_LC_MISSING];
                        textField.TextColor = styles[(int)CensusColour.CENSUS_PRESENT_LC_MISSING];
                        textField.ToolTip = "Census entered but no Lost Cousins flag set.";
                        break;
                    case "CENSUS_PRESENT_NOT_LC_YEAR":
                        textField.BackgroundColor = styles[(int)CensusColour.CENSUS_PRESENT_NOT_LC_YEAR];
                        textField.TextColor = styles[(int)CensusColour.CENSUS_PRESENT_NOT_LC_YEAR];
                        textField.ToolTip = "Census entered and not a Lost Cousins year.";
                        break;
                    case "CENSUS_PRESENT_LC_PRESENT":
                        textField.BackgroundColor = styles[(int)CensusColour.CENSUS_PRESENT_LC_PRESENT];
                        textField.TextColor = styles[(int)CensusColour.CENSUS_PRESENT_LC_PRESENT];
                        textField.ToolTip = "Census entered and flagged as entered on Lost Cousins.";
                        break;
                    case "LC_PRESENT_NO_CENSUS":
                        textField.BackgroundColor = styles[(int)CensusColour.LC_PRESENT_NO_CENSUS];
                        textField.TextColor = styles[(int)CensusColour.LC_PRESENT_NO_CENSUS];
                        textField.ToolTip = "Lost Cousins flagged but no Census entered. {SearchTooltip}";
                        break;
                    case "OVERSEAS_CENSUS":
                        textField.BackgroundColor = styles[(int)CensusColour.OVERSEAS_CENSUS];
                        textField.TextColor = styles[(int)CensusColour.OVERSEAS_CENSUS];
                        textField.ToolTip = $"On Census outside {Country}.";
                        break;
                    case "OUT_OF_COUNTRY":
                        textField.BackgroundColor = styles[(int)CensusColour.OUT_OF_COUNTRY];
                        textField.TextColor = styles[(int)CensusColour.OUT_OF_COUNTRY];
                        textField.ToolTip = $"Likely outside {Country} on census date. {SearchTooltip}";
                        break;
                    case "KNOWN_MISSING":
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