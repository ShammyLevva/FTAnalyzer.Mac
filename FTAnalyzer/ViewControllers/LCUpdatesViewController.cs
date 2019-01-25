using System;
using System.Collections.Generic;
using AppKit;

namespace FTAnalyzer
{
    public partial class LCUpdatesViewController : NSViewController
	{
        List<CensusIndividual> LCUpdates;
        List<CensusIndividual> LCInvalidRef;

		public LCUpdatesViewController (IntPtr handle) : base (handle)
		{
		}

        public void UpdateLostCousinsReport(RelationshipTypesView relationshipTypes)
        {
            InvokeOnMainThread(() =>
            {
                Predicate<CensusIndividual> relationFilter = relationshipTypes.BuildFilter<CensusIndividual>(x => x.RelationType, true);
                LCUpdates = new List<CensusIndividual>();
                LCInvalidRef = new List<CensusIndividual>();
                string reportText = FamilyTree.Instance.LCOutput(LCUpdates, LCInvalidRef, relationFilter);
                StatsTextbox.Value = reportText;
            });
        }
    }
}
