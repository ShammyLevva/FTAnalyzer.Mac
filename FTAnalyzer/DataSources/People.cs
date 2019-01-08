using System;
using System.Collections.Generic;
using AppKit;
using Foundation;
using FTAnalyzer.Utilities;
using FTAnalyzer.ViewControllers;

namespace FTAnalyzer
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
            peopleWindow.Window.SetFrame(new CoreGraphics.CGRect(300,300,800,500),true);
            peopleView = peopleWindow.ContentViewController as PeopleViewController;
         }

        public void SetWorkers(string job, SortableBindingList<Individual> workers)
        {
            peopleWindow.Window.Title = "Individuals whose occupation was " + (job.Length == 0 ? "not entered" : job);
            SortableBindingList<IDisplayIndividual> dsInd = new SortableBindingList<IDisplayIndividual>();
            foreach (Individual i in workers)
                dsInd.Add(i);
            peopleView.LoadIndividuals(dsInd);
            peopleView.HideFamilies(true);
            UpdateStatusCount();
        }

        public void SetIndividuals(List<Individual> individuals, string reportTitle)
        {
            peopleWindow.Window.Title = reportTitle;
            peopleView.LoadIndividuals(new SortableBindingList<IDisplayIndividual>(individuals));
            peopleView.HideFamilies(true);
            UpdateStatusCount();
        }

        public void SetLocation(FactLocation loc, int level)
        {
            peopleWindow.Window.Title = "Individuals & Families with connection to " + loc.ToString();
            level = Math.Min(loc.Level, level); // if location level isn't as detailed as level on tab use location level
            IEnumerable<Individual> listInd = FamilyTree.Instance.GetIndividualsAtLocation(loc, level);
            SortableBindingList<IDisplayIndividual> dsInd = new SortableBindingList<IDisplayIndividual>();
            foreach (Individual i in listInd)
                dsInd.Add(i);
            peopleView.LoadIndividuals(dsInd);

            IEnumerable<Family> listFam = FamilyTree.Instance.GetFamiliesAtLocation(loc, level);
            SortableBindingList<IDisplayFamily> dsFam = new SortableBindingList<IDisplayFamily>();
            foreach (Family f in listFam)
                dsFam.Add(f);
            peopleView.LoadFamilies(dsFam);
            UpdateStatusCount();
        }

        public void ShowWindow(NSObject sender)
        {
            peopleWindow.ShowWindow(sender);
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
