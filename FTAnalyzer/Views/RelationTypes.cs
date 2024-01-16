using FTAnalyzer.Filters;

namespace FTAnalyzer
{
    public partial class RelationTypes : NSView
    {
        public bool Directs => DirectOutlet.State == NSCellStateValue.On;
        public bool Blood => BloodOutlet.State == NSCellStateValue.On;
        public bool Marriage => MarriageOutlet.State == NSCellStateValue.On;
        public bool MarriedToDB { get => MarriedDBOutlet.State == NSCellStateValue.On; set => MarriedDBOutlet.State = value ? NSCellStateValue.On : NSCellStateValue.Off; }
        public bool Unknown => UnknownOutlet.State == NSCellStateValue.On;
        public bool Descendant => DescendantsOutlet.State == NSCellStateValue.On;
        public bool Linked => LinkedOutlet.State == NSCellStateValue.On;

        public RelationTypes(IntPtr handle) : base(handle)
        {
        }

        public RelationTypes()
        {   // This one is temporary should be replaced by relationshiptypes view for colour census report

            DirectOutlet = new NSButton();
            BloodOutlet = new NSButton();
            MarriageOutlet = new NSButton();
            MarriedDBOutlet = new NSButton();
            UnknownOutlet = new NSButton();
            DescendantsOutlet = new NSButton();
            LinkedOutlet = new NSButton();

            DirectOutlet.State = NSCellStateValue.On;
            BloodOutlet.State = NSCellStateValue.On;
            MarriageOutlet.State = NSCellStateValue.Off;
            MarriedDBOutlet.State = NSCellStateValue.On;
            UnknownOutlet.State = NSCellStateValue.Off;
            DescendantsOutlet.State = NSCellStateValue.On;
            LinkedOutlet.State = NSCellStateValue.On;
        }

        public Predicate<T> BuildFilter<T>(Func<T, int> relationType, bool excludeUnknown = false)
        {

            List<Predicate<T>> relationFilters = new();
            InvokeOnMainThread(() =>
            {
                if (Blood)
                    relationFilters.Add(FilterUtils.IntFilter(relationType, Individual.BLOOD));
                if (Directs)
                    relationFilters.Add(FilterUtils.IntFilter(relationType, Individual.DIRECT));
                if (Marriage)
                    relationFilters.Add(FilterUtils.IntFilter(relationType, Individual.MARRIAGE));
                if (MarriedToDB)
                    relationFilters.Add(FilterUtils.IntFilter(relationType, Individual.MARRIEDTODB));
                if (Descendant)
                    relationFilters.Add(FilterUtils.IntFilter(relationType, Individual.DESCENDANT));
                if (Linked)
                    relationFilters.Add(FilterUtils.IntFilter(relationType, Individual.LINKED));
                if (UnknownOutlet.State == NSCellStateValue.On && !excludeUnknown)
                    relationFilters.Add(FilterUtils.IntFilter(relationType, Individual.UNKNOWN));
            });
            return FilterUtils.OrFilter(relationFilters);
        }

        public Predicate<Family> BuildFamilyFilter<Family>(Func<Family, IEnumerable<int>> relationTypes)
        {
            List<Predicate<Family>> relationFilters = new();
            if (Blood)
                relationFilters.Add(FilterUtils.FamilyRelationFilter(relationTypes, Individual.BLOOD));
            if (Directs)
                relationFilters.Add(FilterUtils.FamilyRelationFilter(relationTypes, Individual.DIRECT));
            if (Marriage)
                relationFilters.Add(FilterUtils.FamilyRelationFilter(relationTypes, Individual.MARRIAGE));
            if (MarriedToDB)
                relationFilters.Add(FilterUtils.FamilyRelationFilter(relationTypes, Individual.MARRIEDTODB));
            if (Descendant)
                relationFilters.Add(FilterUtils.FamilyRelationFilter(relationTypes, Individual.DESCENDANT));
            if (Descendant)
                relationFilters.Add(FilterUtils.FamilyRelationFilter(relationTypes, Individual.LINKED));
            if (Unknown)
                relationFilters.Add(FilterUtils.FamilyRelationFilter(relationTypes, Individual.UNKNOWN));
            return FilterUtils.OrFilter(relationFilters);
        }

        partial void BloodChecked(NSObject sender) => RelationshipTypesChanged();

        partial void ByMarriageChecked(NSObject sender) => RelationshipTypesChanged();

        partial void DescendantsChecked(NSObject sender) => RelationshipTypesChanged();

        partial void DirectChecked(NSObject sender) => RelationshipTypesChanged();

        partial void LinkedChecked(NSObject sender) => RelationshipTypesChanged();

        partial void MarriedDBChecked(NSObject sender) => RelationshipTypesChanged();

        partial void UnknownChecked(NSObject sender) => RelationshipTypesChanged();

        void RelationshipTypesChanged()
        {

        }
    }
}
