using System;
using AppKit;
using Foundation;
using FTAnalyzer.Mac.DataSources;

namespace FTAnalyzer.Mac.ViewControllers
{
    public class LocationViewController : BindingListViewController<IDisplayLocation>
    {
        public LocationViewController(string title, string tooltip) : base(title, tooltip)
        {
        }

        [Export("ViewDetailsSelector")]
        public override void ViewDetailsSelector()
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(ViewDetailsSelector);
                return;
            }
            NSTableColumn column = GetColumnID("IndividualID");
            column = GetColumnID("Country");
            if (column != null)
            {
                var source = _tableView.Source as BindingListTableSource<IDisplayLocation>;
                var location = source.GetRowObject(_tableView.SelectedRow) as FactLocation;
                if (location != null)
                {
                    People people = new People();
                    switch (_tableView.Identifier)
                    {
                        case "Countries":
                            people.SetLocation(location, FactLocation.COUNTRY);
                            break;
                        case "Regions":
                            people.SetLocation(location, FactLocation.REGION);
                            break;
                        case "Sub-Regions":
                            people.SetLocation(location, FactLocation.SUBREGION);
                            break;
                        case "Addresses":
                            people.SetLocation(location, FactLocation.ADDRESS);
                            break;
                        case "Places":
                            people.SetLocation(location, FactLocation.PLACE);
                            break;
                    }
                    RaiseLocationRowClicked(people);
                    return;
                }
            }

        }

        public delegate void LocationRowClickedDelegate(People people);
        public event LocationRowClickedDelegate LocationRowClicked;

        internal void RaiseLocationRowClicked(People people)
        {
            LocationRowClicked?.Invoke(people);
        }
    }
}
