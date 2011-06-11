namespace Opuno.Brenn.Website.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Opuno.Brenn.Sync;

    [TestClass]
    public class ServiceTest
    {
        private static SyncResponse GetSyncResponseBlocking(SyncRequest request)
        {
            var responseLock = new System.Threading.AutoResetEvent(false);
            var response = default(SyncResponse);

            ServiceHelper.Instance.Queue<SyncRequest, SyncResponse>(
                "Sync",
                r =>
                {
                    response = r;
                    responseLock.Set();
                },
                request);

            responseLock.WaitOne();

            return response;
        }

        [TestMethod]
        public void ChangeExpense1()
        {
            SyncResponse response;

            response = GetSyncResponseBlocking(new SyncRequest() { ClientChangeSetN = 0 });

            var expense = response.Expenses.First();
            var serverChangeSet = expense.ChangeSetN;

            expense.DisplayName = "Changed name";
            var originalRowId = expense.RowId;
            expense.RowId = Guid.NewGuid();

            response =
                GetSyncResponseBlocking(
                    new SyncRequest
                        {
                            ClientChangeSetN = serverChangeSet,
                            ExpenseUpdates =
                                new List<ExpenseUpdate> { new ExpenseUpdate() { ClientRowId = originalRowId, Expense = expense } }
                        });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ExpenseUpdateResults);
            Assert.AreEqual(1, response.ExpenseUpdateResults.Count);
            Assert.IsNotNull(response.Expenses);
            Assert.AreEqual(0, response.Expenses.Count);

            var expenseUpdateResult = response.ExpenseUpdateResults.SingleOrDefault();

            Assert.IsNotNull(expenseUpdateResult);
            Assert.AreEqual(expense.RowId, expenseUpdateResult.ClientRowId);
            Assert.IsNull(expenseUpdateResult.Error);
        }

        [TestMethod]
        public void FailChangeExpenseBecauseModified()
        {
            SyncResponse response;

            response = GetSyncResponseBlocking(new SyncRequest() { ClientChangeSetN = 0 });

            var expense = response.Expenses.First();
            var serverChangeSet = response.ServerChangeSetN;

            expense.DisplayName = "Changed name";
            expense.RowId = Guid.NewGuid();

            // The client row id is wrong and the update should fail.
            response =
                GetSyncResponseBlocking(
                    new SyncRequest
                    {
                        ClientChangeSetN = serverChangeSet,
                        ExpenseUpdates =
                            new List<ExpenseUpdate> { new ExpenseUpdate() { ClientRowId = Guid.NewGuid(), Expense = expense } }
                    });

            serverChangeSet++;
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ExpenseUpdateResults);
            Assert.AreEqual(1, response.ExpenseUpdateResults.Count);
            Assert.IsNotNull(response.Expenses);
            Assert.AreEqual(0, response.Expenses.Count);
            Assert.AreEqual(serverChangeSet, response.ServerChangeSetN);

            var expenseUpdateResult = response.ExpenseUpdateResults.SingleOrDefault();

            Assert.IsNotNull(expenseUpdateResult);
            Assert.AreEqual(expense.RowId, expenseUpdateResult.ClientRowId);
            Assert.AreEqual(UpdateError.ModifiedOnServer, expenseUpdateResult.Error);

            response =
                GetSyncResponseBlocking(
                    new SyncRequest
                    {
                        ClientChangeSetN = serverChangeSet,
                        ExpenseUpdates =
                            new List<ExpenseUpdate>()
                    });

            Assert.AreEqual(0, response.Expenses.Count);
            Assert.AreEqual(0, response.Trips.Count);
            Assert.AreEqual(0, response.People.Count);
            Assert.AreEqual(serverChangeSet, response.ServerChangeSetN);
        }

        [TestMethod]
        public void FailChangeExpenseBecauseDeleted()
        {
            SyncResponse response;

            response = GetSyncResponseBlocking(new SyncRequest() { ClientChangeSetN = 0 });

            var expense = response.Expenses.First();
            var serverChangeSet = expense.ChangeSetN;

            expense.DisplayName = "Changed name";
            expense.RowId = Guid.NewGuid();
            expense.ExpenseId = 487537845;

            // The client row id is wrong and the update should fail.
            response =
                GetSyncResponseBlocking(
                    new SyncRequest
                    {
                        ClientChangeSetN = serverChangeSet,
                        ExpenseUpdates =
                            new List<ExpenseUpdate> { new ExpenseUpdate() { ClientRowId = Guid.NewGuid(), Expense = expense } }
                    });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ExpenseUpdateResults);
            Assert.AreEqual(1, response.ExpenseUpdateResults.Count);
            Assert.IsNotNull(response.Expenses);
            Assert.AreEqual(0, response.Expenses.Count);

            var expenseUpdateResult = response.ExpenseUpdateResults.SingleOrDefault();

            Assert.IsNotNull(expenseUpdateResult);
            Assert.AreEqual(expense.RowId, expenseUpdateResult.ClientRowId);
            Assert.AreEqual(UpdateError.DeletedOnServer, expenseUpdateResult.Error);
        }
    }
}
