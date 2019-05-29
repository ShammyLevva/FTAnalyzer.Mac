﻿using System;
using System.Collections.Generic;
using AppKit;
using FTAnalyzer.Utilities;
using FTAnalyzer.ViewControllers;
using static FTAnalyzer.ColourValues;

namespace FTAnalyzer.DataSources
{
    public class ColourBMDSource : BindingListTableSource<IDisplayColourBMD>
    {
        readonly Dictionary<int, NSColor> styles;
        string BMDProvider { get; }
        string SearchTooltip { get; }

        public ColourBMDSource(string bmdProvider, SortableBindingList<IDisplayColourBMD> reportList)
            : base(reportList)
        {
            BMDProvider = bmdProvider;
            SearchTooltip = $"Double click to search {BMDProvider}.";
            styles = new Dictionary<int, NSColor>();
            NSColor notRequired = Color.DarkGray;
            styles.Add(0, notRequired);
            NSColor missingData = Color.Red;
            styles.Add(1, missingData);
            NSColor openEndedDateRange = Color.OrangeRed;
            styles.Add(13, openEndedDateRange);
            NSColor verywideDateRange = Color.Tomato;
            styles.Add(2, verywideDateRange);
            NSColor wideDateRange = Color.Orange;
            styles.Add(3, wideDateRange);
            NSColor narrowDateRange = Color.Yellow;
            styles.Add(5, narrowDateRange);
            NSColor justYear = Color.YellowGreen;
            styles.Add(4, justYear);
            NSColor approxDate = Color.PaleGreen;
            styles.Add(6, approxDate);
            NSColor exactDate = Color.LawnGreen;
            styles.Add(7, exactDate);

            NSColor nospouse = Color.LightPink;
            styles.Add(8, nospouse);
            NSColor hasChildren = Color.LightCoral;
            styles.Add(9, hasChildren);
            NSColor noMarriage = Color.Firebrick;
            styles.Add(10, noMarriage);
            NSColor isLiving = Color.WhiteSmoke;
            styles.Add(11, isLiving);
            NSColor over90 = Color.DarkGray;
            styles.Add(12, over90);
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var index = Array.IndexOf(_fieldNames, tableColumn.Identifier);
            Console.WriteLine($"Index: {index}, Column: {tableColumn.Title}, Hidden: {tableColumn.Hidden}, Header: {tableColumn.HeaderCell.Title}");
            NSTableCellView cellView = GetFTAnalyzerGridCell(tableView, tableColumn, row);
            if (index >= ColourBMDViewController.BMDColumnsStart && index <= ColourBMDViewController.BMDColumnsEnd)
            {
                SetTextField(cellView.TextField, tableColumn, row);
                var textField = cellView.TextField;
                switch (textField.IntValue)
                {
                    case (int)BMDColour.EMPTY:
                        textField.BackgroundColor = styles[(int)BMDColour.EMPTY];
                        textField.TextColor = styles[(int)BMDColour.EMPTY];
                        if (index == ColourBMDViewController.BMDColumnsEnd || index == ColourBMDViewController.BMDColumnsEnd / -1)
                            textField.ToolTip = "Individual is probably still alive";
                        else
                            textField.ToolTip = string.Empty;
                        break;
                    case (int)BMDColour.UNKNOWN_DATE:
                        textField.BackgroundColor = styles[(int)BMDColour.UNKNOWN_DATE];
                        textField.TextColor = styles[(int)BMDColour.UNKNOWN_DATE];
                        textField.ToolTip = "Unknown date.";
                        break;
                    case (int)BMDColour.OPEN_ENDED_DATE:
                        textField.BackgroundColor = styles[(int)BMDColour.OPEN_ENDED_DATE];
                        textField.TextColor = styles[(int)BMDColour.OPEN_ENDED_DATE];
                        textField.ToolTip = "Date is open ended, BEFore or AFTer a date.";
                        break;
                    case (int)BMDColour.VERY_WIDE_DATE:
                        textField.BackgroundColor = styles[(int)BMDColour.VERY_WIDE_DATE];
                        textField.TextColor = styles[(int)BMDColour.VERY_WIDE_DATE];
                        textField.ToolTip = "Date only accurate to more than ten year date range.";
                        break;
                    case (int)BMDColour.WIDE_DATE:
                        textField.BackgroundColor = styles[(int)BMDColour.WIDE_DATE];
                        textField.TextColor = styles[(int)BMDColour.WIDE_DATE];
                        textField.ToolTip = "Date covers up to a ten year date range."; 
                        break;
                    case (int)BMDColour.NARROW_DATE:
                        textField.BackgroundColor = styles[(int)BMDColour.NARROW_DATE];
                        textField.TextColor = styles[(int)BMDColour.NARROW_DATE];
                        textField.ToolTip = "Date accurate to within one to two year period.";
                        break;
                    case (int)BMDColour.JUST_YEAR_DATE:
                        textField.BackgroundColor = styles[(int)BMDColour.JUST_YEAR_DATE];
                        textField.TextColor = styles[(int)BMDColour.JUST_YEAR_DATE];
                        textField.ToolTip = "Date accurate to within one year period, but longer than 3 months.";
                        break;
                    case (int)BMDColour.APPROX_DATE:
                        textField.BackgroundColor = styles[(int)BMDColour.APPROX_DATE];
                        textField.TextColor = styles[(int)BMDColour.APPROX_DATE];
                        textField.ToolTip = "Date accurate to within 3 months (note may be date of registration not event date)";
                        break;
                    case (int)BMDColour.EXACT_DATE:
                        textField.BackgroundColor = styles[(int)BMDColour.EXACT_DATE];
                        textField.TextColor = styles[(int)BMDColour.EXACT_DATE];
                        textField.ToolTip = "Exact Date.";
                        break;
                    case (int)BMDColour.NO_SPOUSE:
                        textField.BackgroundColor = styles[(int)BMDColour.NO_SPOUSE];
                        textField.TextColor = styles[(int)BMDColour.NO_SPOUSE];
                        textField.ToolTip = "Of marrying age but no spouse recorded";
                        break;
                    case (int)BMDColour.NO_PARTNER:
                        textField.BackgroundColor = styles[(int)BMDColour.NO_PARTNER];
                        textField.TextColor = styles[(int)BMDColour.NO_PARTNER];
                        textField.ToolTip = "No partner but has shared fact or children";
                        break;
                    case (int)BMDColour.NO_MARRIAGE:
                        textField.BackgroundColor = styles[(int)BMDColour.NO_MARRIAGE];
                        textField.TextColor = styles[(int)BMDColour.NO_MARRIAGE];
                        textField.ToolTip = "Has partner but no marriage fact";
                        break;
                    case (int)BMDColour.ISLIVING:
                        textField.BackgroundColor = styles[(int)BMDColour.ISLIVING];
                        textField.TextColor = styles[(int)BMDColour.ISLIVING];
                        textField.ToolTip = "Is flagged as living";
                        break;
                    case (int)BMDColour.OVER90:
                        textField.ToolTip = "Individual may be still alive";
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
                if (index >= ColourBMDViewController.BMDColumnsStart && index <= ColourBMDViewController.BMDColumnsEnd)
                    view.IntValue = (int)propertyValue;
                else
                    view.StringValue = propertyValue == null ? string.Empty : propertyValue.ToString();
            }
            else
                view.StringValue = string.Empty;
        }

    }
}
