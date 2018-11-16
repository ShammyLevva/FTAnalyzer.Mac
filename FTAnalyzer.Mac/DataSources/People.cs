using System.Collections.Generic;
using AppKit;
using Foundation;
using FTAnalyzer.Utilities;

namespace FTAnalyzer.Mac
{
    public class People
    {
        enum ReportType { People, MissingChildrenStatus, MismatchedChildrenStatus }
        NSWindowController peopleWindow;
        PeopleViewController peopleView;

        public People()
        {
            var storyboard = NSStoryboard.FromName("People", null);
            peopleWindow = storyboard.InstantiateControllerWithIdentifier("PeopleWindow") as NSWindowController;
            peopleView = peopleWindow.ContentViewController as PeopleViewController;
         }

        public void SetWorkers(string job, SortableBindingList<Individual> workers)
        {
            peopleWindow.Window.Title = "Individuals whose occupation was " + (job.Length == 0 ? "not entered" : job);
            SortableBindingList<IDisplayIndividual> dsInd = new SortableBindingList<IDisplayIndividual>();
            foreach (Individual i in workers)
                dsInd.Add(i);
            peopleView.RefreshIndividuals(dsInd);
            SortIndividuals();
            peopleView.HideFamilies();
            UpdateStatusCount();
        }

        public void SetIndividuals(List<Individual> individuals, string reportTitle)
        {
            peopleWindow.Window.Title = reportTitle;
            peopleView.RefreshIndividuals(new SortableBindingList<IDisplayIndividual>(individuals));
            peopleView.HideFamilies();
            UpdateStatusCount();
        }

        public void ShowWindow(NSObject sender)
        {
            peopleWindow.ShowWindow(sender);
        }

        void SortIndividuals()
        {
           // dgIndividuals.Sort(dgIndividuals.Columns[1], ListSortDirection.Ascending);
           // dgIndividuals.Sort(dgIndividuals.Columns[2], ListSortDirection.Ascending);
        }

        void SortFamilies()
        {
           // dgFamilies.Sort(dgFamilies.Columns[0], ListSortDirection.Ascending);
        }

        void UpdateStatusCount()
        {
            //if (reportType == ReportType.MissingChildrenStatus || reportType == ReportType.MismatchedChildrenStatus)
            //    txtCount.Text = $"{dgFamilies.RowCount} Problems detected. " + Properties.Messages.Hints_IndividualFamily + " Shift Double click to see colour census report for family.";
            //else
            //{
            //   if (splitContainer.Panel2Collapsed)
            //        txtCount.Text = "Count: " + dgIndividuals.RowCount + " Individuals.  " + Properties.Messages.Hints_Individual;
            //    else
            //        txtCount.Text = "Count: " + dgIndividuals.RowCount + " Individuals and " + dgFamilies.RowCount + " Families. " + Properties.Messages.Hints_IndividualFamily;
            //}
        }

        void ResetTable()
        {
            if (peopleView.IsIndividualViewVisible)
                peopleView.SortIndividuals();
            if (peopleView.IsFamilyViewVisible)
                peopleView.SortFamilies();
        }


    }
}
