using System;
using System.Collections.Generic;
using AppKit;
using Foundation;

namespace FTAnalyzer
{
    public interface IPrintViewController
    {
        string Title { get; }
        NSTableViewSource TableSource { get; }
        NSSortDescriptor[] SortDescriptors { get; }
        Type GetGenericType();
        Dictionary<string, float> ColumnWidths();
    }
}
