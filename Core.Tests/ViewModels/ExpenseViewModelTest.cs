namespace Opuno.Brenn.Core.Tests.ViewModels
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Opuno.Brenn.Models;
    using Opuno.Brenn.ViewModels;

    [TestClass]
    public class ExpenseViewModelTest
    {
        [TestMethod]
        public void EditExpense()
        {
            var person1 = new Person { ChangeSetN = 1, DisplayName = "Bobby", PersonId = 1, RowId = Guid.NewGuid() };
            var person2 = new Person { ChangeSetN = 1, DisplayName = "Sophie", PersonId = 2, RowId = Guid.NewGuid() };
            var person3 = new Person { ChangeSetN = 1, DisplayName = "Glenn", PersonId = 3, RowId = Guid.NewGuid() };

            var trip = new Trip
                {
                    ChangeSetN = 1,
                    DisplayName = "Paris 2011",
                    RowId = Guid.NewGuid(),
                    TripId = 1
                };

            var expense = new Expense
                {
                    Amount = 100,
                    ChangeSetN = 1,
                    DisplayName = "Beers",
                    ExpenseId = 1,
                    Sender = person1,
                    Receivers = new List<Person> { person1, person2 },
                    RecordDate = new DateTime(2011, 01, 02, 15, 30, 00),
                    RowId = Guid.NewGuid(),
                    Trip = trip
                };

            var oldExpenseRowId = expense.RowId;

            var viewModel = new ExpenseViewModel { Model = expense };

            viewModel.LoadFromModel();

            // Switch the payer from person1 to person3 (and also receiver)
            viewModel.Sender = person3;
            viewModel.Receivers.Remove(person1);
            viewModel.Receivers.Add(person3);

            viewModel.SaveToModel();

            Assert.AreEqual(100, expense.Amount);
            Assert.AreEqual(1, expense.ExpenseId);
            Assert.AreEqual(expense.Sender, person3);
            Assert.AreEqual(2, expense.Receivers.Count);
            Assert.IsTrue(expense.Receivers.Contains(person2));
            Assert.IsTrue(expense.Receivers.Contains(person3));
            Assert.AreEqual(new DateTime(2011, 01, 02, 15, 30, 00), expense.RecordDate);
            Assert.AreNotEqual(oldExpenseRowId, expense.RowId);
            Assert.AreEqual(expense.Trip, trip);
        }

        [TestMethod]
        public void NewExpense()
        {
            var person1 = new Person { ChangeSetN = 1, DisplayName = "Bobby", PersonId = 1, RowId = Guid.NewGuid() };
            var person2 = new Person { ChangeSetN = 1, DisplayName = "Sophie", PersonId = 2, RowId = Guid.NewGuid() };
            var person3 = new Person { ChangeSetN = 1, DisplayName = "Glenn", PersonId = 3, RowId = Guid.NewGuid() };

            var viewModel = new ExpenseViewModel
                {
                    Sender = person1,
                    RecordDate = new DateTime(2010, 1, 3, 19, 25, 0),
                    Amount = 150,
                    DisplayName = "Expense Uno"
                };

            viewModel.Receivers.Add(person2);
            viewModel.Receivers.Add(person3);

            viewModel.SaveToModel();

            var model = viewModel.Model;

            Assert.IsNotNull(model);
            Assert.AreEqual(person1, model.Sender);
            Assert.AreEqual(2, model.Receivers.Count);
            Assert.IsTrue(model.Receivers.Contains(person2));
            Assert.IsTrue(model.Receivers.Contains(person3));
            Assert.AreEqual(150, model.Amount);
            Assert.AreEqual("Expense Uno", model.DisplayName);
            Assert.AreEqual(new DateTime(2010, 1, 3, 19, 25, 0), model.RecordDate);
            Assert.AreEqual(default(int), model.ChangeSetN);
            Assert.AreNotEqual(default(Guid), model.RowId);
            Assert.AreEqual(default(int), model.ExpenseId);
        }
    }
}
