namespace Opuno.Brenn.Core.Tests.ViewModels
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Opuno.Brenn.Models;
    using Opuno.Brenn.ViewModels;

    [TestClass]
    public class TripViewModelTest
    {
        [TestMethod]
        public void EditTripViewModel()
        {
            var trip = new Trip
                {
                    ChangeSetN = 1,
                    DisplayName = "Trip to Paris",
                    RowId = Guid.NewGuid(),
                    TripId = 1,
                    Expenses = new List<Expense>()
                };

            var oldRowId = trip.RowId;
            var tripViewModel = new TripViewModel { Model = trip };

            tripViewModel.LoadFromModel();

            Assert.AreEqual(trip.DisplayName, tripViewModel.DisplayName);
            Assert.AreEqual(trip.Expenses.Count, tripViewModel.Expenses.Count);

            tripViewModel.DisplayName = "Trip to Paris/Rome";
            tripViewModel.SaveToModel();

            Assert.AreNotEqual(oldRowId, trip.RowId);
            Assert.AreEqual(trip.DisplayName, tripViewModel.DisplayName);
        }

        [TestMethod]
        public void EditTripViewModelWithNoChanges()
        {
            var trip = new Trip
            {
                ChangeSetN = 1,
                DisplayName = "Trip to Paris",
                RowId = Guid.NewGuid(),
                TripId = 1,
                Expenses = new List<Expense>()
            };

            var oldRowId = trip.RowId;
            var tripViewModel = new TripViewModel { Model = trip };

            tripViewModel.LoadFromModel();

            Assert.AreEqual(trip.DisplayName, tripViewModel.DisplayName);
            Assert.AreEqual(trip.Expenses.Count, tripViewModel.Expenses.Count);

            tripViewModel.SaveToModel();

            Assert.AreEqual(oldRowId, trip.RowId, "The row id should not change unless changes were made.");
            Assert.AreEqual(trip.DisplayName, tripViewModel.DisplayName);
        }

        [TestMethod]
        public void NewTripViewModel()
        {
            var vm = new TripViewModel { DisplayName = "Misc stuff" };

            // User clicks "Save"
            vm.SaveToModel();

            var model = vm.Model;

            Assert.AreEqual(vm.DisplayName, model.DisplayName);
            Assert.AreNotEqual(default(Guid), model.RowId, "The view should create a new row identifier if it changed the model.");
        }
    }
}
