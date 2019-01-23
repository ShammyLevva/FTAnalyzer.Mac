using System;
using System.Collections.Generic;
using AppKit;

namespace FTAnalyzer
{
    public partial class LCUpdatesViewController : NSViewController
	{
        List<CensusIndividual> LCUpdates;

		public LCUpdatesViewController (IntPtr handle) : base (handle)
		{
		}

        public void UpdateLostCousinsReport(RelationshipTypesView relationshipTypes)
        {
            Predicate<CensusIndividual> relationFilter = relationshipTypes.BuildFilter<CensusIndividual>(x => x.RelationType, true);
            LCUpdates = new List<CensusIndividual>();
            StatsTextbox.Value = FamilyTree.Instance.LCOutput(LCUpdates, relationFilter);
        }
    }
}
