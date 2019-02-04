using System;
using System.Collections.Generic;
using AppKit;
using FTAnalyzer.Filters;

namespace FTAnalyzer.ViewControllers
{
    public class RelationTypes : NSViewController
    {
        public bool Directs => true;
        public bool Blood => true; //ckbBlood.Checked;
        public bool Marriage => false; //ckbMarriage.Checked;
        public bool MarriedToDB => true; //{ get => ckbMarriageDB.Checked; set => ckbMarriageDB.Checked = value; }
        public bool Unknown => false; //ckbUnknown.Checked;
        public bool Descendant => true; //ckbDescendants.Checked;
        public bool Linked => true;

        public int Status
        {
            get
            {
                int result = 0;
                if (Unknown)
                   result += Individual.UNKNOWN;
                if (Directs)
                    result += Individual.DIRECT;
                if (Blood)
                    result += Individual.BLOOD;
                if (Marriage)
                    result += Individual.MARRIAGE;
                if (MarriedToDB)
                    result += Individual.MARRIEDTODB;
                if (Descendant)
                    result += Individual.DESCENDANT;
                if (Linked)
                    result += Individual.LINKED;
                return result;
            }
        }
        public Predicate<T> BuildFilter<T>(Func<T, int> relationType)
        {
            List<Predicate<T>> relationFilters = new List<Predicate<T>>();
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
            if (Unknown)
                relationFilters.Add(FilterUtils.IntFilter(relationType, Individual.UNKNOWN));
            return FilterUtils.OrFilter(relationFilters);
        }

        public Predicate<Family> BuildFamilyFilter<Family>(Func<Family, IEnumerable<int>> relationTypes)
        {
            List<Predicate<Family>> relationFilters = new List<Predicate<Family>>();
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
            if (Unknown)
                relationFilters.Add(FilterUtils.FamilyRelationFilter(relationTypes, Individual.UNKNOWN));
            return FilterUtils.OrFilter(relationFilters);
        }
    }
}
