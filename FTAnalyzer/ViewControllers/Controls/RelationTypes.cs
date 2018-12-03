using System;
using System.Collections.Generic;
using AppKit;
using FTAnalyzer.Filters;

namespace FTAnalyzer.ViewControllers
{
    public class RelationTypes : NSViewController
    {
        public bool Directs => true; // ckbDirects.Checked;
        public bool Blood => true; //ckbBlood.Checked;
        public bool Marriage => false; //ckbMarriage.Checked;
        public bool MarriedToDB => true; //{ get => ckbMarriageDB.Checked; set => ckbMarriageDB.Checked = value; }
        public bool Unknown => false; //ckbUnknown.Checked;
        public bool Descendant => true; //ckbDescendants.Checked;

        public int Status
        {
            get
            {
                int result = Individual.DIRECT + Individual.DESCENDANT + Individual.MARRIEDTODB + Individual.BLOOD; //0;
                //if (ckbUnknown.Checked)
                //   result += Individual.UNKNOWN;
                //if (ckbDirects.Checked)
                //    result += Individual.DIRECT;
                //if (ckbBlood.Checked)
                //    result += Individual.BLOOD;
                //if (ckbMarriage.Checked)
                //    result += Individual.MARRIAGE;
                //if (ckbMarriageDB.Checked)
                //    result += Individual.MARRIEDTODB;
                //if (ckbDescendants.Checked)
                //result += Individual.DESCENDANT;
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
