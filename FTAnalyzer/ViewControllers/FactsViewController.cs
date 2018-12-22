﻿using Foundation;
using FTAnalyzer.ViewControllers;
using FTAnalyzer.Utilities;
using System.Collections.Generic;
using System.Linq;
using FTAnalyzer.DataSources;

namespace FTAnalyzer
{
    public partial class FactsViewController<T>: BindingListViewController<T>, IPrintViewController
	{
        readonly SortableBindingList<IDisplayFact> facts;

        public FactsViewController(string title, Individual individual) : base (title, string.Empty)
		{
            facts = new SortableBindingList<IDisplayFact>();
            AddIndividualsFacts(individual);
            RefreshDocumentView();
		}

        public FactsViewController(string title, Family family) : base(title, string.Empty)
        {
            facts = new SortableBindingList<IDisplayFact>();
            AddFamilyFacts(family);
            RefreshDocumentView();
        }

        public FactsViewController(string title, FactSource source) : base(title, string.Empty)
        {
            facts = FamilyTree.Instance.GetSourceDisplayFacts(source);
            Title = $"{title}.Facts count: {facts.Count}";
            RefreshDocumentView();
        }

        void RefreshDocumentView()
        {
            if (!NSThread.IsMain)
            {
                InvokeOnMainThread(RefreshDocumentView);
                return;
            }

            var source = new BindingListTableSource<IDisplayFact>(facts);
            SetViews(source as BindingListTableSource<T>);
        }

        void AddIndividualsFacts(Individual individual)
        {
            IEnumerable<Fact> list = individual.AllFacts.Union(individual.ErrorFacts.Where(f => f.FactErrorLevel != Fact.FactError.WARNINGALLOW));
            foreach (Fact f in list)
                facts.Add(new DisplayFact(individual, f));
        }

        void AddFamilyFacts(Family family)
        {
            foreach (DisplayFact f in family.AllDisplayFacts)
                facts.Add(f);
        }
    }
}
