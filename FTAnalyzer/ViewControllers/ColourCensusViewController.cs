using System;
using AppKit;
using Foundation;
using FTAnalyzer.DataSources;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.ViewControllers
{
    public partial class ColourCensusViewController : BindingListViewController<IDisplayColourCensus>
    {
        public string Country { get; }
        public static int CensusColumnsStart => 5; // GetColumnIndex("C1841"); zero based index
        public static int CensusColumnsEnd => 49; //  GetColumnIndex("V1925");
        int startColumnIndex;
        int endColumnIndex;
        int CensusProviderIndex { get; }
        string CensusProvider { get; }
        string CensusRegion { get; }

        public ColourCensusViewController(IntPtr handle) : base(string.Empty, string.Empty)
        {
        }

        public ColourCensusViewController(string country, int providerIndex, string censusRegion) : base("Census Research Suggestions", string.Empty)
        {
            Country = country;
            SetColumns();
            CensusProviderIndex = providerIndex;
            CensusRegion = censusRegion;
            CensusProvider = FamilyTree.ProviderName(providerIndex);
        }

        public override void RefreshDocumentView(SortableBindingList<IDisplayColourCensus> list)
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(() => RefreshDocumentView(list));
                return;
            }
            CountText = $"Count: {list.Count}";
            UpdateTooltip();
            _tableView.AutosaveName = string.Empty; // don't autosave as screws up different countries
            _tableView.AutosaveTableColumns = false;
            _tableView.Source = new ColourCensusSource(Country, startColumnIndex, endColumnIndex, CensusProvider, list);
            _tableView.ReloadData();
            _tableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.None;
            Title = $"Census Research Suggestions for {Country}. {list.Count} records listed.";
        }

        void SetColumns()
        {
            if (Country.Equals(Countries.UNITED_STATES))
            {
                startColumnIndex = GetColumnIndex("US1790");
                endColumnIndex = GetColumnIndex("US1940");
                // cbFilter.Items[5] = "Outside USA (Dark Grey)";
            }
            else if (Country.Equals(Countries.CANADA))
            {
                startColumnIndex = GetColumnIndex("Can1851");
                endColumnIndex = GetColumnIndex("Can1921");
                // cbFilter.Items[5] = "Outside Canada (Dark Grey)";
            }
            else if (Country.Equals(Countries.IRELAND))
            {
                startColumnIndex = GetColumnIndex("Ire1901");
                endColumnIndex = GetColumnIndex("Ire1911");
                // cbFilter.Items[5] = "Outside Ireland (Dark Grey)";
            }
            else if (Country.Equals(Countries.UNITED_KINGDOM))
            {
                startColumnIndex = GetColumnIndex("C1841");
                endColumnIndex = GetColumnIndex("C1939");
                // cbFilter.Items[5] = "Outside UK (Dark Grey)";
            }
            else
                Console.WriteLine("We have a problem.");

            for (int index = CensusColumnsStart; index <= CensusColumnsEnd; index++)
            {
                NSTableColumn column = _tableView.TableColumns().GetValue(index) as NSTableColumn;
                if (index >= startColumnIndex && index <= endColumnIndex)
                {
                    column.Hidden = false;
                    column.Width = 40;
                    column.MinWidth = 40;
                    column.MaxWidth = 40;
                    column.HeaderCell.Alignment = NSTextAlignment.Center;
                }
                else
                    column.Hidden = true;
            }
        }

        [Export("ViewDetailsSelector")]
        public override void ViewDetailsSelector()
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(ViewDetailsSelector);
                return;
            }
            nint row = _tableView.ClickedRow;
            int columnIndex = (int)_tableView.ClickedColumn;
            if (row >= 0 && columnIndex >= startColumnIndex && columnIndex <= endColumnIndex)
            {
                var source = _tableView.Source as ColourCensusSource;
                IDisplayColourCensus person = source.GetRowObject(row) as IDisplayColourCensus;
                int censusYear;
                if (Country == Countries.UNITED_STATES)
                    censusYear = 1790 + (columnIndex - startColumnIndex) * 10;
                else if (Country == Countries.CANADA)
                    censusYear = columnIndex <= GetColumnIndex("Can1901")
                        ? 1851 + (columnIndex - startColumnIndex) * 10
                        : 1901 + (columnIndex - GetColumnIndex("Can1901")) * 5;
                else if (Country.Equals(Countries.IRELAND))
                    censusYear = 1901 + (columnIndex - startColumnIndex) * 10;
                else
                    censusYear = columnIndex == GetColumnIndex("1939") ? 1939 : 1841 + (columnIndex - startColumnIndex) * 10;
                string censusCountry = person.BestLocation(new FactDate(censusYear.ToString())).CensusCountry;
                if (censusYear == 1939 && CensusProvider != "Find My Past" && CensusProvider != "Ancestry")
                    UIHelpers.ShowMessage($"Unable to search the 1939 National Register on {CensusProvider}.", "FTAnalyzer");
                else
                {
                    try
                    {
                        var ft = FamilyTree.Instance;
                        FamilyTree.SearchCensus(censusCountry, censusYear, ft.GetIndividual(person.IndividualID), CensusProviderIndex, CensusRegion);
                    }
                    catch (CensusSearchException ex)
                    {
                        UIHelpers.ShowMessage(ex.Message);
                    }
                }
            }
        }
    }
}
